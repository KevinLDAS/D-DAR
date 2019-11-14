using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace EasyProfile
{
    public class PopupViewController : MonoBehaviour
    {
        [SerializeField]
        private Text TitleTXT = default;
        [SerializeField]
        private Text BodyTXT = default;

        private Action OnYesCallback;
        private Action OnNoCallback;

        public void OnYesClick()
        {
            if (OnYesCallback != null)
            {
                OnYesCallback.Invoke();
            }
            ResetMSG();
            EasyProfileManager.Instance.VIEW_CONTROLLER.HidePopupMessage();
        }

        public void OnNoClick()
        {
            if (OnNoCallback != null)
            {
                OnNoCallback.Invoke();
            }
            ResetMSG();
            EasyProfileManager.Instance.VIEW_CONTROLLER.HidePopupMessage();
        }

        public void SetupMSG(PopupMSG _msg)
        {
            ResetMSG();
            TitleTXT.text = _msg.Title;
            BodyTXT.text = _msg.Body;
            OnYesCallback = _msg.OnYesCallback;
            OnNoCallback = _msg.OnNoCallback;
        }

        private void ResetMSG()
        {
            TitleTXT.text = string.Empty;
            BodyTXT.text = string.Empty;
            OnYesCallback = null;
            OnNoCallback = null;
        }
    }

    public class PopupMSG
    {
        public string Title;
        public string Body;
        public Action OnYesCallback;
        public Action OnNoCallback;
    }

}

