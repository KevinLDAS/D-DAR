using UnityEngine;
using UnityEngine.UI;

namespace EasyProfile
{
    public class UserViewController : MonoBehaviour
    {
        // UI elements
        [SerializeField]
        private Text FullNameLabel = default;
        [SerializeField]
        private Text NickNameLabel = default;
        [SerializeField]
        private Text DateCreationLabel = default;
        [SerializeField]
        private Text UserIDLabel = default;
        [SerializeField]
        private Text CoinsLabel = default;
        [SerializeField]
        private Text ExpLabel = default;
        [SerializeField]
        private Text StatusLabel = default;
        [SerializeField]
        private AvatarViewController AvatarController = default;
        [SerializeField]
        private GameObject LoadFromCameraButton = default;
        [SerializeField]
        private GameObject LoadFromGalleryButton = default;
        [SerializeField]
        private GameObject LoadFromPCButton = default;

        // Cached user object
        private User LoadedUser = default;

        /// <summary>
        /// Start method
        /// </summary>
        private void Start()
        {
            Init();
        }

        /// <summary>
        /// OnDestroy method. Remove listeners
        /// </summary>
        private void OnDestroy()
        {
            if (EasyProfileManager.Instance != null)
            {
                EasyProfileManager.Instance.RemoveOnUploadCompleteAction(OnImageUploaded);
            }
        }

        /// <summary>
        /// OnEnable method. Start load users data
        /// </summary>
        private void OnEnable()
        {
            GetUserData();
            AvatarController.LoadAvatar(EasyProfileManager.Instance.PROFILE_CONTROLLER.USER_ID);
        }

        /// <summary>
        /// Init view controller
        /// </summary>
        private void Init()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                LoadFromPCButton.SetActive(false);
            }
            else
            {
                LoadFromCameraButton.SetActive(false);
                LoadFromGalleryButton.SetActive(false);
            }
            EasyProfileManager.Instance.AddOnUploadAvatarCompleteAction(OnImageUploaded);
            EasyProfileManager.Instance.GetCustomValue("Gold", OnCustomDataGetted);
            EasyProfileManager.Instance.GetCustomValue("Exp", OnCustomDataGetted);
            EasyProfileManager.Instance.GetCustomValue("Status", OnCustomDataGetted);
        }

        /// <summary>
        /// Callback for custom data getted
        /// </summary>
        /// <param name="_response"></param>
        public void OnCustomDataGetted(CallbackGetUserCustomValue _response)
        {
            if (_response.ValueID == "Gold")
            {
                int _gold = _response.CustomValue.IntValue;
                CoinsLabel.text = _gold.ToString();
            }
            if (_response.ValueID == "Exp")
            {
                float _exp = _response.CustomValue.FloatValue;
                ExpLabel.text = _exp.ToString();
            }
            if (_response.ValueID == "Status")
            {
                string _status = _response.CustomValue.StringValue;
                StatusLabel.text = _status;
            }
        }

        /// <summary>
        /// Start get user data
        /// </summary>
        public void GetUserData()
        {
            EasyProfileManager.Instance.GetUserData(OnUserDataGetted);
        }

        /// <summary>
        /// Callback for image upload complete
        /// </summary>
        /// <param name="_callback"></param>
        private void OnImageUploaded(CallBackUploadImage _callback)
        {
            EasyProfileManager.Instance.PROFILE_CONTROLLER.PROFILE_IMAGE_LOADED = false;
            AvatarController.LoadAvatar(EasyProfileManager.Instance.PROFILE_CONTROLLER.USER_ID);
        }

        /// <summary>
        /// Callback for user data getted
        /// </summary>
        /// <param name="_callbak"></param>
        private void OnUserDataGetted(CallbackGetUserDataMessage _callbak)
        {
            LoadedUser = _callbak.UserData;
            DisplayUserData();
        }

        /// <summary>
        /// Display user data when loaded
        /// </summary>
        public void DisplayUserData()
        {
            if (LoadedUser != null)
            {
                if (FullNameLabel != null) FullNameLabel.text = LoadedUser.FullName;
                if (NickNameLabel != null) NickNameLabel.text = LoadedUser.NickName;
                if (DateCreationLabel != null) DateCreationLabel.text = LoadedUser.DataRegistration;
                if (UserIDLabel != null) UserIDLabel.text = LoadedUser.UserID;
            }
            else
            {
                Debug.LogError("Can not load display user data. User is null");
            }
        }

        /// <summary>
        /// Button click event for open device photo gallery
        /// </summary>
        public void TakeImageFromGallery()
        {
            EasyProfileManager.Instance.PROFILE_SOLUTION.UploadAvatarFromGallery();
        }

        /// <summary>
        /// Button click event for open device camera
        /// </summary>
        public void TakeImageFromCamera()
        {
            EasyProfileManager.Instance.PROFILE_SOLUTION.UploadAvatarFromCamera();
        }

        /// <summary>
        /// Button click event for get image from PC
        /// </summary>
        public void TakeImageFromPC()
        {
            EasyProfileManager.Instance.PROFILE_SOLUTION.UploadAvatarFromPC();
        }

        /// <summary>
        /// Button click event log out user
        /// </summary>
        public void OnLogOut()
        {
            EasyProfileManager.Instance.LogOut();
            EasyProfileManager.Instance.VIEW_CONTROLLER.HideUserProfileView();
        }
    }
}



