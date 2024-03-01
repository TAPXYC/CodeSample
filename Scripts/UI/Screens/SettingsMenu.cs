using System;
using Fungus;
using Gravitons.UI.Modal;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static event Action<ColorTheme> OnThemeChange;
    public static ColorTheme CurrentTheme { get; private set; }
    public static bool SoundOn { get; private set; }
    public static bool NotifyOn { get; private set; }


    [SerializeField] Switch notifySwitch;
    [SerializeField] Switch soundSwitch;
    // [SerializeField] MultiSwitch languageSwitch;
    [SerializeField] MultiSwitch themeSwitch;
    [Space]
    [SerializeField] Button informationButton;
    [SerializeField] Button confidentialButton;
    [Space]
    [SerializeField] Button clearCashButton;
    [Space]
    [SerializeField] Button connectAccountButton;
    [SerializeField] Button signOutButton;
    [SerializeField] Button deleteAccountButton;
    [Space]
    [SerializeField] Button vkButton;
    [SerializeField] Button tgButton;
    [SerializeField] Button webButton;
    [SerializeField] Button fbButton;
    [Space]
    [SerializeField] Button closeButton;
    [Space]
    [SerializeField] Animation anim;

    public event Action OnShowSignIn;

    private const string _themeSaveKey = "theme";
    private const string _soundSaveKey = "sound";
    private const string _notifySaveKey = "notify";
    private FirebaseController _firebaseController;

    public void Init(FirebaseController firebaseController)
    {
        _firebaseController = firebaseController;
        _firebaseController.Auth.OnCompleteLogin += (m, u) => UpdateVisual(u != null);

        NotifyOn = PlayerPrefs.GetInt(_notifySaveKey, 1) == 1;
        notifySwitch.Init(NotifyOn);
        notifySwitch.OnChangeValue += SetNotify;

        SoundOn = PlayerPrefs.GetInt(_soundSaveKey, 1) == 1;
        soundSwitch.Init(SoundOn);
        soundSwitch.OnChangeValue += SetSound;
        var musicManager = FungusManager.Instance.MusicManager;
        musicManager.SetAudioVolume(SoundOn ? 1 : 0, 0, null);

        themeSwitch.Init("theme");
        themeSwitch.OnClick += ChangeThemeClick;

        connectAccountButton.onClick.AddListener(ShowSignIn);
        signOutButton.onClick.AddListener(SignOut);
        deleteAccountButton.onClick.AddListener(DeleteAccount);

        confidentialButton.onClick.AddListener(() => Application.OpenURL("https://butterflyeffectstories.ru/privacypolicy"));
        informationButton.onClick.AddListener(ShowSupportWindow);

        clearCashButton.onClick.AddListener(ClearCash);

        vkButton.onClick.AddListener(() => Application.OpenURL("https://vk.com/butterflyeffectstories"));
        tgButton.onClick.AddListener(() => Application.OpenURL("https://t.me/Butterfly_effect_story"));
        webButton.onClick.AddListener(() => Application.OpenURL("https://butterflyeffectstories.ru"));
        fbButton.onClick.AddListener(() => Application.OpenURL("https://www.facebook.com/groups/316578178005470"));

        closeButton.onClick.AddListener(CloseMenu);

        gameObject.SetActive(false);

        ColorTheme currentTheme = (ColorTheme)PlayerPrefs.GetInt(_themeSaveKey, 0);
        ChangeTheme(currentTheme);
    }

    private void SetSound(bool isOn)
    {
        SoundOn = isOn;
        PlayerPrefs.SetInt(_soundSaveKey, SoundOn ? 1 : 0);

        var musicManager = FungusManager.Instance.MusicManager;
        musicManager.SetAudioVolume(SoundOn ? 1 : 0, 0.5f, null);
    }

    private void SetNotify(bool isOn)
    {
        NotifyOn = isOn;
        PlayerPrefs.SetInt(_notifySaveKey, NotifyOn ? 1 : 0);
    }

    private void ClearCash()
    {
        ModalManager.Show("Кэш очищен", "Временные данные успешно удалены", new ModalButton[] { new ModalButton("Ок") });
    }

    private void UpdateVisual(bool authMode)
    {
        if (authMode)
        {
            connectAccountButton.transform.parent.gameObject.SetActive(false);
            signOutButton.transform.parent.gameObject.SetActive(true);
            deleteAccountButton.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            connectAccountButton.transform.parent.gameObject.SetActive(true);
            signOutButton.transform.parent.gameObject.SetActive(false);
            deleteAccountButton.transform.parent.gameObject.SetActive(false);
        }
    }

    private void DeleteAccount()
    {
        ModalManager.Show("Вы уверены?", "В случае удаления аккаунта все данные будут утеряны", new ModalButton[] {
                                                                                                                new ModalButton("Удалить", _firebaseController.Auth.DeleteAccount),
                                                                                                                new ModalButton("Отмена") });
    }

    private void SignOut()
    {
        ModalManager.Show("Выйти из аккаунта", "Подтвердите действие", new ModalButton[] {
                                                                                        new ModalButton("Да", _firebaseController.Auth.SighOut),
                                                                                        new ModalButton("Отмена") });
    }

    private void ShowSignIn()
    {
        OnShowSignIn?.Invoke();
    }

    private void ShowSupportWindow()
    {
        if (_firebaseController.Auth.IsLoggin)
        {
            string UID = _firebaseController.Auth.AuthUserData.ID;
            ModalManager.Show("Информация", $"Ваш UID - {UID}", new ModalButton[] {
                                                                                new ModalButton("Скопировать", () => GUIUtility.systemCopyBuffer = UID),
                                                                                new ModalButton("Назад") });
        }
        else
        {
            ModalManager.Show("Информация", "Вы не авторизованы", new ModalButton[] { new ModalButton("Закрыть") });
        }
    }

    private void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    public void ShowMenu()
    {
        gameObject.SetActive(true);
        anim.Play();
    }


    public static void ChangeTheme(ColorTheme theme)
    {
        if (theme != CurrentTheme)
        {
            CurrentTheme = theme;
            PlayerPrefs.SetInt(_themeSaveKey, (int)CurrentTheme);
            OnThemeChange?.Invoke(CurrentTheme);
        }
    }


    private void ChangeThemeClick(int themeIndex)
    {
        ChangeTheme((ColorTheme)themeIndex);
        notifySwitch.ChangeBodyColor();
        soundSwitch.ChangeBodyColor();
    }
}
