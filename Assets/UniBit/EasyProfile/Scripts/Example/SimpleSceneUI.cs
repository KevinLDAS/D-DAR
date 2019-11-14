using UnityEngine;

namespace EasyProfile
{
    public class SimpleSceneUI : MonoBehaviour
    {
        private bool IsWindowsActive = false;

        public void ShowProfileWindows()
        {
            IsWindowsActive = !IsWindowsActive;
            if (IsWindowsActive)
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowUserProfileView();
            }
            else
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.HideUserProfileView();
            }
        }
    }
}
