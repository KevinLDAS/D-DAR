using UnityEngine;
using UnityEngine.UI;

namespace EasyProfile
{
    public class RegistrationViewController : MonoBehaviour
    {
        // UI email input field
        [SerializeField]
        private InputField EmailInput = default;
        // UI first name input field
        [SerializeField]
        private InputField FirstNameInput = default;
        // UI last name input field
        [SerializeField]
        private InputField LastNameInput = default;
        // UI nick name input field
        [SerializeField]
        private InputField NickNameInput = default;
        // UI password input field
        [SerializeField]
        private InputField PasswordInput = default;
        // UI confirm password input field
        [SerializeField]
        private InputField ConfirmPasswordInput = default;

        /// <summary>
        /// OnEnable method
        /// </summary>
        private void OnEnable()
        {
            ClearFields();
        }

        /// <summary>
        /// Registration button click event
        /// </summary>
        public void SendRegistration()
        {
            if (CheckError())
                return;
            string _login = EmailInput.text.Trim();
            string _password = PasswordInput.text.Trim();
            EasyProfileManager.Instance.RegisterNewUser(_login, _password, OnRegistrationComplete);
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowLoading();
        }

        /// <summary>
        /// Close registration window
        /// </summary>
        public void CloseWindow()
        {
            EasyProfileManager.Instance.VIEW_CONTROLLER.HideRegistration();
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowLogin();
        }

        /// <summary>
        /// Callback for registration complete
        /// </summary>
        /// <param name="_msg"></param>
        public void OnRegistrationComplete(CallbackRegistrationMessage _msg)
        {
            if (_msg.IsComplete)
            {
                string _login = EmailInput.text.Trim();
                string _password = PasswordInput.text.Trim();
                User _user = new User();
                _user.UserID = _msg.UserID;
                _user.DataRegistration = EasyProfileManager.Instance.GetSystemDate();
                _user.FirstName = FirstNameInput.text;
                _user.LastName = LastNameInput.text;
                _user.NickName = NickNameInput.text;
                _user.FullName = FirstNameInput.text + " " + LastNameInput.text;
                EasyProfileManager.Instance.SetUserData(_user, OnSetUserDataComplete);
            }
            else
            {
                PopupMessage msg = new PopupMessage();
                msg.Title = "Error";
                msg.Message = _msg.ErrorMessage;
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMessage(msg);
                EasyProfileManager.Instance.VIEW_CONTROLLER.HideLoading();
            }
        }

        /// <summary>
        /// Callback for set user data complete
        /// </summary>
        /// <param name="_msg"></param>
        public void OnSetUserDataComplete(CallbackSetUserDataMessage _msg)
        {
            if (_msg.IsSuccess)
            {
                if (EasyProfileManager.Instance.PROFILE_SETTING.UseEmailConfirm)
                {
                    EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMSG(ProfileMessageCode.RegistrationSuccessWithConfirm, CloseWindow);
                    EasyProfileManager.Instance.SendMailVerification();
                }
                else
                {
                    EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMSG(ProfileMessageCode.RegistrationSuccess, CloseWindow);
                }
                EasyProfileManager.Instance.VIEW_CONTROLLER.HideLoading();
            }
            else
            {
                PopupMessage msg = new PopupMessage();
                msg.Title = "Error";
                msg.Message = _msg.ErrorMessage;
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMessage(msg);
                EasyProfileManager.Instance.VIEW_CONTROLLER.HideLoading();
            }
        }

        /// <summary>
        /// Reset all UI input fields
        /// </summary>
        private void ClearFields()
        {
            EmailInput.text = string.Empty;
            FirstNameInput.text = string.Empty;
            LastNameInput.text = string.Empty;
            PasswordInput.text = string.Empty;
            ConfirmPasswordInput.text = string.Empty;
            NickNameInput.text = string.Empty;
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
            if (string.IsNullOrEmpty(FirstNameInput.text))
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMSG(ProfileMessageCode.EmptyFirstName);
                IsError = true;
            }
            if (string.IsNullOrEmpty(LastNameInput.text))
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMSG(ProfileMessageCode.EmptyLastName);
                IsError = true;
            }
            if (string.IsNullOrEmpty(NickNameInput.text))
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMSG(ProfileMessageCode.EmptyNickName);
                IsError = true;
            }
            else if (string.IsNullOrEmpty(PasswordInput.text))
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMSG(ProfileMessageCode.EmptyPassword);
                IsError = true;
            }
            else if (!string.Equals(ConfirmPasswordInput.text, ConfirmPasswordInput.text))
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMSG(ProfileMessageCode.PasswordNotMatch);
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


