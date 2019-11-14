using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace EasyProfile
{
    public class LoginViewController : MonoBehaviour
    {
        // UI email input field
        [SerializeField]
        private InputField EmailInput = default;
        // UI password input field
        [SerializeField]
        private InputField PasswordInput = default;

        /// <summary>
        /// OnEnable method. 
        /// </summary>
        private void OnEnable()
        {
            ClearFields();
        }

        /// <summary>
        /// Login button click event
        /// </summary>
        public void SendLogIn()
        {
            if (CheckError())
                return;
            string _login = EmailInput.text.Trim();
            string _password = PasswordInput.text.Trim();
            OnLogin(_login, _password);
        }

        /// <summary>
        /// On Login method
        /// </summary>
        /// <param name="_mail">User email</param>
        /// <param name="_password">User password</param>
        public void OnLogin(string _mail, string _password)
        {
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowLoading();
            EasyProfileManager.Instance.LogIn(_mail, _password, OnLoginSuccess);
        }

        /// <summary>
        /// Show registration window
        /// </summary>
        public void OnRegistration()
        {
            EasyProfileManager.Instance.VIEW_CONTROLLER.HideLogin();
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowRegistration();
        }

        /// <summary>
        /// Show fogot password window
        /// </summary>
        public void OnFogotPassword()
        {
            EasyProfileManager.Instance.VIEW_CONTROLLER.HideLogin();
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowRestorePassword();
        }

        /// <summary>
        /// Callback for login complete
        /// </summary>
        /// <param name="_msg"></param>
        public void OnLoginSuccess(CallbackLoginMessage _msg)
        {
            if (_msg.IsSuccess)
            {
                if (!string.IsNullOrEmpty(EmailInput.text) && !string.IsNullOrEmpty(PasswordInput.text) && EasyProfileManager.Instance.PROFILE_SETTING.AutoSaveUserCredentials)
                {
                    EasyProfileManager.Instance.SaveUserCredentials(_msg.Gredentials.UserLogin, _msg.Gredentials.UserPassword);
                }

                EasyProfileManager.Instance.GetUserData(_callback =>
                {
                    EasyProfileManager.Instance.PROFILE_CONTROLLER.CURRENT_USER_DATA = _callback.UserData;
                    EasyProfileManager.Instance.VIEW_CONTROLLER.HideLogin();
                    EasyProfileManager.Instance.VIEW_CONTROLLER.HideLoading();
                    if (EasyProfileManager.Instance.PROFILE_SETTING.AllowToLoadScene)
                    {
                        SceneManager.LoadScene(EasyProfileManager.Instance.PROFILE_SETTING.ProfileSceneName);
                    }
                });
            }
            else
            {
                if (_msg.MessageCode == ProfileMessageCode.None)
                {
                    PopupMessage msg = new PopupMessage();
                    msg.Title = "Error";
                    msg.Message = _msg.ErrorMessage;
                    EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMessage(msg);
                    EasyProfileManager.Instance.VIEW_CONTROLLER.HideLoading();
                }
                else
                {
                    EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMSG(ProfileMessageCode.EmailConfirm, EasyProfileManager.Instance.VIEW_CONTROLLER.ShowLogin);
                    EasyProfileManager.Instance.VIEW_CONTROLLER.HideLoading();
                }
                
            }
        }

        /// <summary>
        /// Reset all UI input fields
        /// </summary>
        private void ClearFields()
        {
            EmailInput.text = string.Empty;
            PasswordInput.text = string.Empty;
        }

        /// <summary>
        /// Check inputs for errors
        /// </summary>
        /// <returns></returns>
        private bool CheckError()
        {
            bool IsError = false;
            if (string.IsNullOrEmpty(EmailInput.text))
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMSG(ProfileMessageCode.EmptyEmail);
                IsError = true;
            }
            else if (string.IsNullOrEmpty(PasswordInput.text))
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMSG(ProfileMessageCode.EmptyPassword);
                IsError = true;
            }
            else if (!EmailInput.text.Contains(EasyProfileManager.Instance.PROFILE_SETTING.EmailValidationCharacter))
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMSG(ProfileMessageCode.EmailNotValid);
                IsError = true;
            }
            else if (PasswordInput.text.Length < EasyProfileManager.Instance.PROFILE_SETTING.MinAllowPasswordCharacters)
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMSG(ProfileMessageCode.SmallPassword);
                IsError = true;
            }
            return IsError;
        }
    }
}
