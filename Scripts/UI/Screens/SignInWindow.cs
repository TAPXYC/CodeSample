using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignInWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI errorMessage;
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField password;
    [Space]
    [SerializeField] Button signInButton;
    [SerializeField] Button registrationButton;
    [SerializeField] Button closeButton;

    private AuthController _authController;

    public void Init(FirebaseController firebaseController)
    {
        errorMessage.text = "";
        gameObject.SetActive(false);
        _authController = firebaseController.Auth;

        signInButton.onClick.AddListener(SignIn);
        registrationButton.onClick.AddListener(Register);
        closeButton.onClick.AddListener(CloseMenu);
    }

    public void ShowMenu()
    {
        errorMessage.text = "";
        gameObject.SetActive(true);
    }

    private void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    private void SignIn()
    {
        errorMessage.text = "";

        if (ValidateInput())
            _authController.SignInEmailUser(email.text, password.text, LoginComplete);
    }

    private void Register()
    {
        if (ValidateInput())
            _authController.CreateEmailUser(email.text, password.text, LoginComplete);
    }


    private bool ValidateInput()
    {
        bool success = true;
        if (email.text.IsNullOrEmpty() || password.text.IsNullOrEmpty())
        {
            errorMessage.text = "Заполните все поля";
            success = false;
        }
        else if (password.text.Length < 6)
        {
            errorMessage.text = "Пароль должен быть не менее 6 символов";
            success = false;
        }
        else if (!Regex.IsMatch(email.text, "([a-zA-Z0-9]{1,20})[@]([a-zA-Z0-9]{2,10})[.]([a-zA-Z0-9]{2,10})"))
        {
            errorMessage.text = "Некорректный адрес эл. почты";
            success = false;
        }
        return success;
    }


    private void LoginComplete(AuthMessageData data)
    {
        if (data.Success)
        {
            CloseMenu();
        }
        else
        {
            errorMessage.text = data.Message;
        }
    }


}
