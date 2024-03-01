using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using Firebase;
#if FBON
using Firebase.Auth;
//using Google;
#endif


public struct AuthUserData
{
    public readonly string Name;
    public readonly string ID;
    public readonly string Token;

    public AuthUserData(string name, string iD, string token)
    {
        Name = name;
        ID = iD;
        Token = token;
    }


    public static AuthUserData Empty => new AuthUserData("none", "none", "none");
}


public struct AuthMessageData
{
    public readonly string Message;
    public readonly bool Success;


    public AuthMessageData(string message, bool success)
    {
        Message = message;
        Success = success;
    }
}


public class AuthController
{
    public delegate void AuthStateHandler(AuthMessageData message, AuthUserData? userData);


    public event Action OnBeginLogin;
    public event AuthStateHandler OnCompleteLogin;
    public event Action OnDeleteAccount;


    public AuthUserData AuthUserData;
    public bool IsLoggin => _user != _auth.CurrentUser && _auth.CurrentUser != null && _auth.CurrentUser.IsValid();

    public bool IsLoaded
    {
        get;
        private set;
    } = false;


#if FBON
    private FirebaseAuth _auth;
    private FirebaseUser _user;
#endif
    private MonoBehaviour _owner;
    private CoroutineObject<Action<AuthMessageData>> _googleSignInRoutine;
    private CoroutineObject<Task<AuthResult>, Action<AuthMessageData>> _signInRoutine;
    private CoroutineObject _createUserDataRoutine;
    private CoroutineObject _setAutonomicModeRoutine;

    public AuthController(MonoBehaviour owner)
    {
        _owner = owner;
        _googleSignInRoutine = new CoroutineObject<Action<AuthMessageData>>(_owner, GoogleSignInRoutine);
        _createUserDataRoutine = new CoroutineObject(_owner, CreateUserData);
        _signInRoutine = new CoroutineObject<Task<AuthResult>, Action<AuthMessageData>>(_owner, SignInRoutine);
        _setAutonomicModeRoutine = new CoroutineObject(_owner, SetAutonomicMode);

#if FBON
        _auth = FirebaseAuth.DefaultInstance;
        _auth.StateChanged += AuthStateChanged;
        IsLoaded = _auth.CurrentUser == null;
#endif

        if (IsLoaded)
        {
            _setAutonomicModeRoutine.Start();
        }
    }


    private IEnumerator SetAutonomicMode()
    {
        yield return null;
        AuthMessageData messageData = new AuthMessageData("No login Data. Autonomic mode", false);
        OnCompleteLogin?.Invoke(messageData, null);
    }

    #region EMAIL

    public void CreateEmailUser(string email, string password, Action<AuthMessageData> callback)
    {
#if FBON
        OnBeginLogin?.Invoke();
        _signInRoutine.Start(_auth.CreateUserWithEmailAndPasswordAsync(email, password), callback);
#endif
    }

    public void SignInEmailUser(string email, string password, Action<AuthMessageData> callback)
    {
#if FBON
        OnBeginLogin?.Invoke();
        _signInRoutine.Start(_auth.SignInWithEmailAndPasswordAsync(email, password), callback);
#endif
    }


    private IEnumerator SignInRoutine(Task<AuthResult> task, Action<AuthMessageData> callback)
    {
        Debug.LogWarning("Begin sign in");
#if FBON
        bool complete = false;
        AuthMessageData messageData = new AuthMessageData();

        task.ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Вход был отменен");
                messageData = new AuthMessageData("Вход был отменен", false);
            }
            else if (task.IsFaulted)
            {
                string error = GetErrorMessage(task.Exception);
                Debug.LogError(error);
                messageData = new AuthMessageData(error, false);
            }
            else
            {
                AuthResult result = task.Result;
                Debug.LogFormat("Аккаунт создан: {0} ({1})", result.User.DisplayName, result.User.UserId);
                messageData = new AuthMessageData($"Аккаунт создан", true);
            }
        }).ContinueWith(task => complete = true);

        yield return new WaitUntil(() => complete);
        Debug.LogWarning($"Sign in {messageData.Message}");
        callback?.Invoke(messageData);
#endif
    }

    #endregion




    public void SighOut()
    {
#if FBON
        _auth.SignOut();
        AuthUserData = AuthUserData.Empty;
        _setAutonomicModeRoutine.Start();
#endif
    }


    public void DeleteAccount()
    {
#if FBON
        if (_auth.CurrentUser != null)
        {
            OnDeleteAccount?.Invoke();
            _auth.CurrentUser.DeleteAsync();
            AuthUserData = AuthUserData.Empty;
            _setAutonomicModeRoutine.Start();
        }
#endif
    }


    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
#if FBON
        if (_auth.CurrentUser != _user)
        {
            if (!IsLoggin && _user != null)
            {
                Debug.Log("Signed out " + _user.UserId);
            }

            _user = _auth.CurrentUser;

            if (IsLoggin)
            {
                Debug.Log("Signed in ");
                _createUserDataRoutine.Start();
            }
        }
#endif
    }



    private IEnumerator CreateUserData()
    {
#if FBON
        bool complete = false;
        AuthMessageData messageData = new AuthMessageData();
        AuthUserData? userData = null;

        _user.TokenAsync(true).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Вход отменен");
                messageData = new AuthMessageData("Вход отменен", false);
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Не удалось войти в аккаунт (возможно удален)" + task.Exception);
                messageData = new AuthMessageData("Не удалось войти в аккаунт (возможно удален)", false);
            }
            else
            {
                AuthUserData = new AuthUserData(_user.DisplayName ?? "no name", _user.UserId, task.Result);
                Debug.Log($"Успешно зашёл {_user.UserId}  ({task.Result})");
                messageData = new AuthMessageData("Успешно зашёл " + _user.UserId, true);
                userData = AuthUserData;
            }
        }).ContinueWith(task => complete = true);

        yield return new WaitUntil(() => complete);
        OnCompleteLogin?.Invoke(messageData, userData);
        IsLoaded = true;
#endif
    }



    public static string GetErrorMessage(Exception exception)
    {
        FirebaseException firebaseEx = exception.InnerException.InnerException as FirebaseException;

        if (firebaseEx != null)
        {
            var errorCode = (AuthError)firebaseEx.ErrorCode;
            return GetErrorMessage(errorCode);
        }

        return exception.ToString();
    }



    private static string GetErrorMessage(AuthError errorCode)
    {
        var message = "";
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Учетная запись уже существует с другими учетными данными";
                break;
            case AuthError.MissingPassword:
                message = "Отсутствует пароль";
                break;
            case AuthError.WeakPassword:
                message = "Слабый пароль";
                break;
            case AuthError.WrongPassword:
                message = "Неверный пароль";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Учетная запись с таким адресом электронной почты уже существует";
                break;
            case AuthError.InvalidEmail:
                message = "Неверная почта";
                break;
            case AuthError.MissingEmail:
                message = "Отсутствует электронная почта";
                break;
            case AuthError.UserNotFound:
                message = "Не найден аккаунт";
                break;
            default:
                message = "Ошибка";
                break;
        }
        return message;
    }



    public void OnDestroy()
    {
#if FBON
        _auth.StateChanged -= AuthStateChanged;
        _auth = null;
#endif
    }












#region Google

    private const string _googleWebAPI = "380447242095-mdb6fdvp0ig3pv3m4rffbrdjoh8ujtbf.apps.googleusercontent.com";

    public void GoogleSignIn(Action<AuthMessageData> callback)
    {
#if FBON
        _googleSignInRoutine.Start(callback);
#endif
    }


    private IEnumerator GoogleSignInRoutine(Action<AuthMessageData> callback)
    {
        yield return null;
        /*
        Debug.LogWarning("Create");
#if FBON
        bool complete = false;
        AuthMessageData messageData = new AuthMessageData();
        GoogleSignInConfiguration googleConf = new GoogleSignInConfiguration()
        {
            WebClientId = _googleWebAPI,
            RequestIdToken = true
        };

        yield return new WaitForSeconds(1);
        Debug.LogWarning("set conf");

        Google.GoogleSignIn.Configuration = googleConf;
        Google.GoogleSignIn.Configuration.UseGameSignIn = false;
        Google.GoogleSignIn.Configuration.RequestIdToken = true;
        Google.GoogleSignIn.Configuration.RequestEmail = true;
        yield return new WaitForSeconds(3);
        Debug.LogWarning("begin sign in");

        Google.GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Fault");
                messageData = new AuthMessageData("Fault google", false);
                complete = true;
            }
            else if (task.IsCanceled)
            {
                Debug.LogError("Canceled");
                messageData = new AuthMessageData("Canceled google", false);
                complete = true;
            }
            else
            {
                Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
                _auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task =>
                    {
                        if (task.IsCanceled)
                        {
                            Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                            messageData = new AuthMessageData("SignIn was canceled.", false);
                        }
                        else if (task.IsFaulted)
                        {
                            Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                            messageData = new AuthMessageData("SignIn encountered an error: " + task.Exception, false);
                        }
                        else
                        {
                            AuthResult result = task.Result;
                            Debug.LogFormat("User signed in successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
                            messageData = new AuthMessageData($"Firebase user created successfully: {result.User.DisplayName} ({result.User.UserId})", true);
                        }
                    }).ContinueWith(task => complete = true);

            }
        });

        yield return new WaitUntil(() => complete);
        Debug.LogWarning($"Sign in {messageData.Message}");
        callback?.Invoke(messageData);
#endif*/
    }
#endregion


}
