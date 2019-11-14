using UnityEngine;
using System;

namespace EasyProfile
{
    public class ProfileViewController : MonoBehaviour
    {
        // Login windows view object
        private GameObject LoginViewObject;
        // Registration windows view object
        private GameObject RegistrationViewObject;
        // Loading windows view object
        private GameObject LoadingViewObject;
        // Popup windows view object
        private GameObject PopupViewObject;
        // Reset password windows view object
        private GameObject ResetPasswordViewObject;
        // User windows view object
        private GameObject UserViewObject;

        /// <summary>
        /// Show registration view window
        /// </summary>
        public void ShowRegistration()
        {
            if (RegistrationViewObject == null)
            {
                RegistrationViewObject = Instantiate(EasyProfileManager.Instance.PROFILE_SETTING.RegistrationWindowPrefab, transform);
            }
            RegistrationViewObject.SetActive(true);
        }

        /// <summary>
        /// Hide registration view window
        /// </summary>
        public void HideRegistration()
        {
            if (RegistrationViewObject != null)
                RegistrationViewObject.SetActive(false);
        }

        /// <summary>
        /// Show login view window
        /// </summary>
        public void ShowLogin()
        {
            if (LoginViewObject == null)
            {
                LoginViewObject = Instantiate(EasyProfileManager.Instance.PROFILE_SETTING.LoginWindowPrefab, transform);
            }
            LoginViewObject.SetActive(true);
        }

        /// <summary>
        /// Hide login view window
        /// </summary>
        public void HideLogin()
        {
            if (LoginViewObject != null)
                LoginViewObject.SetActive(false);
        }

        /// <summary>
        /// Show restore password window
        /// </summary>
        public void ShowRestorePassword()
        {
            if (ResetPasswordViewObject == null)
            {
                ResetPasswordViewObject = Instantiate(EasyProfileManager.Instance.PROFILE_SETTING.RestorePasswordWindowPrefab, transform);
            }
            ResetPasswordViewObject.SetActive(true);
        }

        /// <summary>
        /// Hide restore password window
        /// </summary>
        public void HideRestorePassword()
        {
            if (ResetPasswordViewObject != null)
                ResetPasswordViewObject.SetActive(false);
        }

        /// <summary>
        /// Show loading view windows
        /// </summary>
        public void ShowLoading()
        {
            if (LoadingViewObject == null)
            {
                LoadingViewObject = Instantiate(EasyProfileManager.Instance.PROFILE_SETTING.LoadingWindowPrefab, transform);
            }
            LoadingViewObject.SetActive(true);
        }

        /// <summary>
        /// Hide loading view window
        /// </summary>
        public void HideLoading()
        {
            if (LoadingViewObject != null)
                LoadingViewObject.SetActive(false);
        }

        /// <summary>
        /// Show user profile view window
        /// </summary>
        public void ShowUserProfileView()
        {
            if (UserViewObject == null)
            {
                UserViewObject = Instantiate(EasyProfileManager.Instance.PROFILE_SETTING.UserProfileWindowPrefab, transform);
            }
            UserViewObject.SetActive(true);
        }

        /// <summary>
        /// Hide user profile view window
        /// </summary>
        public void HideUserProfileView()
        {
            if (UserViewObject != null)
                UserViewObject.SetActive(false);
        }

        /// <summary>
        /// Show popup view window
        /// </summary>
        /// <param name="_msg">PopugMessage object</param>
        public void ShowPopupMessage(PopupMessage _msg)
        {
            if (PopupViewObject == null)
            {
                PopupViewObject = Instantiate(EasyProfileManager.Instance.PROFILE_SETTING.PopupWindowPrefab, transform);
            }
            PopupViewObject.SetActive(true);
            PopupViewObject.GetComponent<PopupController>().ShowMessage(_msg);
        }

        /// <summary>
        /// Hide popup view window
        /// </summary>
        public void HidePopupMessage()
        {
            if (PopupViewObject != null)
                PopupViewObject.SetActive(false);
        }

        /// <summary>
        /// Show popup message by specific code
        /// </summary>
        /// <param name="_code">Pupup message code</param>
        /// <param name="_callback">Add on close pupp method</param>
        public void ShowPopupMSG(ProfileMessageCode _code, Action _callback = null)
        {
            PopupMessage msg = new PopupMessage();
            msg.Callback = _callback;
            switch (_code)
            {
                case ProfileMessageCode.EmptyEmail:
                    msg.Title = "Error";
                    msg.Message = "Email is empty";
                    break;
                case ProfileMessageCode.EmptyFirstName:
                    msg.Title = "Error";
                    msg.Message = "First Name is empty";
                    break;
                case ProfileMessageCode.EmptyLastName:
                    msg.Title = "Error";
                    msg.Message = "Last Name is empty";
                    break;
                case ProfileMessageCode.EmptyNickName:
                    msg.Title = "Error";
                    msg.Message = "Nick Name is empty";
                    break;
                case ProfileMessageCode.EmptyPassword:
                    msg.Title = "Error";
                    msg.Message = "Password is empty";
                    break;
                case ProfileMessageCode.PasswordNotMatch:
                    msg.Title = "Error";
                    msg.Message = "Passwords do not match";
                    break;
                case ProfileMessageCode.EmailNotValid:
                    msg.Title = "Error";
                    msg.Message = "Email is not valid";
                    break;
                case ProfileMessageCode.SmallPassword:
                    msg.Title = "Error";
                    msg.Message = "Password is too small. Min value is " + EasyProfileManager.Instance.PROFILE_SETTING.MinAllowPasswordCharacters.ToString();
                    break;
                case ProfileMessageCode.RegistrationSuccess:
                    msg.Title = "Success";
                    msg.Message = "Registration Success!";
                    break;
                case ProfileMessageCode.RegistrationSuccessWithConfirm:
                    msg.Title = "Success";
                    msg.Message = "Registration Success! Please confirm your email address";
                    break;
                case ProfileMessageCode.EmailConfirm:
                    msg.Title = "Error";
                    msg.Message = "Please confirm your email address";
                    break;
                case ProfileMessageCode.FailedUploadImage:
                    msg.Title = "Error";
                    msg.Message = "Failed Upload Image";
                    break;
                case ProfileMessageCode.RestorePassword:
                    msg.Title = "Success";
                    msg.Message = "Сheck your mail to continue";
                    break;
                default:
                    Debug.Log("NOTHING");
                    break;
            }
            ShowPopupMessage(msg);
        }
    }

    /// <summary>
    /// Enum for popup message code
    /// </summary>
    public enum ProfileMessageCode
    {
        None,
        EmptyEmail,
        EmptyFirstName,
        EmptyLastName,
        EmptyNickName,
        EmptyPassword,
        PasswordNotMatch,
        EmailNotValid,
        SmallPassword,
        RegistrationSuccess,
        RegistrationSuccessWithConfirm,
        EmailConfirm,
        FailedUploadImage,
        RestorePassword,
    }
}


