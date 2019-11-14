using UnityEngine;
using UnityEngine.UI;
using System;

namespace EasyProfile
{
    public class PopupController : MonoBehaviour
    {
        // Message title text label
        [SerializeField]
        private Text TitleLabel = default;
        // Message body text label
        [SerializeField]
        private Text MessageLabel = default;
        // On OK press callback method
        private Action Callback;

        /// <summary>
        /// Display popup message
        /// </summary>
        /// <param name="_msg">PopupMessage object</param>
        public void ShowMessage(PopupMessage _msg)
        {
            TitleLabel.text = _msg.Title;
            MessageLabel.text = _msg.Message;
            Callback = _msg.Callback;
        }

        /// <summary>
        /// Close popup window message
        /// </summary>
        public void CloseWindow()
        {
            if (Callback != null)
            {
                Callback.Invoke();
            }
            EasyProfileManager.Instance.VIEW_CONTROLLER.HidePopupMessage();
        }
    }

    /// <summary>
    /// Popup Message serializable class
    /// </summary>
    [System.Serializable]
    public class PopupMessage
    {
        public ProfileMessageCode MSGCode;
        public string Title;
        public string Message;
        public Action Callback = null;
    }
}
