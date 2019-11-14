using UnityEngine;
using UnityEngine.UI;


namespace EasyProfile
{
    public class FogotPasswordViewController : MonoBehaviour
    {
        // UI Email input field
        [SerializeField]
        private InputField EmailInput = default;

        /// <summary>
        /// On Enable method
        /// </summary>
        private void OnEnable()
        {
            ClearFields();
        }

        /// <summary>
        /// Reset all UI input fields
        /// </summary>
        private void ClearFields()
        {
            EmailInput.text = string.Empty;
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
            return IsError;
        }

        /// <summary>
        /// Send password reset
        /// </summary>
        public void ResetPassword()
        {
            if (CheckError())
                return;
            string _mail = EmailInput.text.Trim();
            EasyProfileManager.Instance.SendResetPasswordEmail(_mail, OnResetPasswordSuccess);
        }

        /// <summary>
        /// On reset password complete method
        /// </summary>
        /// <param name="_response">Response object</param>
        private void OnResetPasswordSuccess(CallbackSendResetPasswordEmail _response)
        {
            if (_response.IsSuccess)
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMSG(ProfileMessageCode.RestorePassword, BackToLogin);
            }
        }

        /// <summary>
        /// Back to login screen
        /// </summary>
        public void BackToLogin()
        {
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowLogin();
            EasyProfileManager.Instance.VIEW_CONTROLLER.HideRestorePassword();
        }
    }
}


