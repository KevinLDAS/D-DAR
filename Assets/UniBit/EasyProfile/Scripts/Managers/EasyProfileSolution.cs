using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleFileBrowser;
using System.IO;

namespace EasyProfile
{
    public class EasyProfileSolution : MonoBehaviour
    {
        /// <summary>
        /// Start method. Init easy profile solution
        /// </summary>
        private void Start()
        {
            Application.targetFrameRate = 60;
            Debug.Log("Remove after video");
            if (!EasyProfileManager.Instance.PROFILE_SETTING.UseApiOnly)
            {
                Init();
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }

        /// <summary>
        /// OnDestroy method. Remove listeners   
        /// </summary>
        private void OnDestroy()
        { 
            if (EasyProfileManager.Instance != null)
            {
                EasyProfileManager.Instance.RemoveOnLogOutAction(OnLogOut);
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        /// <summary>
        /// Init easy profile solution
        /// </summary>
        private void Init()
        {
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowLoading();
            EasyProfileManager.Instance.InitFirebase(OnFirebaseInited);
            EasyProfileManager.Instance.AddOnLogOutAction(OnLogOut);

            NativeCamera.RequestPermission();
            NativeGallery.RequestPermission();
        }

        /// <summary>
        /// Callbak for scene loaded
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == EasyProfileManager.Instance.PROFILE_SETTING.LoginSceneName)
            {
                if (!EasyProfileManager.Instance.PROFILE_SETTING.UseApiOnly)
                {
                    Init();
                }
            }
        }

        /// <summary>
        /// Method for on log event
        /// </summary>
        /// <param name="_callback"></param>
        private void OnLogOut(CallbackLogOut _callback)
        {
            if (EasyProfileManager.Instance.PROFILE_SETTING.AllowToLoadScene)
            {
                EasyProfileManager.Instance.ClearUserCredentials();
                SceneManager.LoadScene(EasyProfileManager.Instance.PROFILE_SETTING.LoginSceneName);
            }
        }

        /// <summary>
        /// Method for on firebase init event
        /// </summary>
        /// <param name="_callback"></param>
        private void OnFirebaseInited(CallbackInitFirebaseMessage _callback)
        {
            EasyProfileManager.Instance.VIEW_CONTROLLER.HideLoading();
            if (_callback.IsSuccess)
            {
                if (EasyProfileManager.Instance.PROFILE_SETTING.AutoLoginOnStart)
                {
                    CheckLogin();
                }
            }
            else
            {
                Debug.LogError("Fail to init Firebase");
            }
        }

        /// <summary>
        /// Check for user credentials
        /// </summary>
        private void CheckLogin()
        {
            if (EasyProfileManager.Instance.HasSavedCredentials())
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowLoading();
                UserCredentials _credentials = EasyProfileManager.Instance.GetSavedCredentials();
                EasyProfileManager.Instance.LogIn(_credentials.UserLogin, _credentials.UserPassword, OnLoginSuccess);
            }
            else
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowLogin();
            }
        }

        /// <summary>
        /// Method for on login complete event
        /// </summary>
        /// <param name="_msg"></param>
        private void OnLoginSuccess(CallbackLoginMessage _msg)
        {
            if (_msg.IsSuccess)
            {
                EasyProfileManager.Instance.GetUserData(_callback=>
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
        /// Callback for image upload complete
        /// </summary>
        /// <param name="_callback"></param>
        private void OnImageUploaded(CallBackUploadImage _callback)
        {
            EasyProfileManager.Instance.VIEW_CONTROLLER.HideLoading();
            if (!_callback.IsSuccess)
            {
                EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMSG(ProfileMessageCode.FailedUploadImage);
            }
        }

        /// <summary>
        /// Resize input Texture
        /// </summary>
        /// <param name="_texture">Current Texture2D</param>
        /// <param name="_size">Image size</param>
        public static void ResizeTexture(Texture2D _texture, ImageSize _size)
        {
            if (_size != ImageSize.Origin)
            {
                int _width = _texture.width;
                int _height = _texture.height;
                if (_width > _height)
                {
                    if (_width > (int)_size)
                    {
                        float _delta = (float)_width / (float)((int)_size);
                        _height = Mathf.FloorToInt((float)_height / _delta);
                        _width = (int)_size;
                    }
                }
                else
                {
                    if (_height > (int)_size)
                    {
                        float _delta = (float)_height / (float)((int)_size);
                        _width = Mathf.FloorToInt((float)_width / _delta);
                        _height = (int)_size;
                    }
                }
                TextureScale.Bilinear(_texture, _width, _height);
            }
        }

        /// <summary>
        /// Callback for success image picked
        /// </summary>
        /// <param name="_path"></param>
        private void OnImagePicked(string _path)
        {
            if (string.IsNullOrEmpty(_path))
                return;
            Texture2D _texture = NativeGallery.LoadImageAtPath(_path, -1, false, false);
            RequestUploadImage uploadRequest = new RequestUploadImage();
            uploadRequest.OwnerId = EasyProfileManager.Instance.PROFILE_CONTROLLER.USER_ID;
            uploadRequest.Texture = _texture;
            EasyProfileManager.Instance.UploadAvatar(uploadRequest, OnImageUploaded);
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowLoading();
        }

        /// <summary>
        /// Callback for success image taked from camera
        /// </summary>
        /// <param name="_path"></param>
        private void OnImageTaken(string _path)
        {
            if (string.IsNullOrEmpty(_path))
                return;
            Texture2D _texture = NativeGallery.LoadImageAtPath(_path, -1, false, false);
            RequestUploadImage uploadRequest = new RequestUploadImage();
            uploadRequest.OwnerId = EasyProfileManager.Instance.PROFILE_CONTROLLER.USER_ID;
            uploadRequest.Texture = _texture;
            EasyProfileManager.Instance.UploadAvatar(uploadRequest, OnImageUploaded);
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowLoading();
        }

        /// <summary>
        /// Callback for success image taked from PC disk
        /// </summary>
        /// <param name="_path"></param>
        private void OnImageLoadFromPC(string _path)
        {
            if (string.IsNullOrEmpty(_path))
                return;
            Texture2D tex = null;
            byte [] fileData = File.ReadAllBytes(_path);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            RequestUploadImage uploadRequest = new RequestUploadImage();
            uploadRequest.OwnerId = EasyProfileManager.Instance.PROFILE_CONTROLLER.USER_ID;
            uploadRequest.Texture = tex;
            EasyProfileManager.Instance.UploadAvatar(uploadRequest, OnImageUploaded);
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowLoading();
        }

        /// <summary>
        /// Callback for failed image load
        /// </summary>
        private void OnImageLoadFailed()
        {

        }

        /// <summary>
        /// Open window for image pick. Work on IOS/Android
        /// </summary>
        public void UploadAvatarFromGallery()
        {
            if (NativeGallery.CheckPermission() != NativeGallery.Permission.Granted)
            {
                NativeGallery.RequestPermission();
            }
            NativeGallery.GetImageFromGallery(OnImagePicked);
        }

        /// <summary>
        /// Open window for image pick. Work on IOS/Android
        /// </summary>
        public void UploadAvatarFromCamera()
        {
            if (NativeCamera.CheckPermission() != NativeCamera.Permission.Granted)
            {
                NativeCamera.RequestPermission();
            }
            NativeCamera.TakePicture(OnImageTaken);
        }

        /// <summary>
        /// Open window for image pick. Work on Windows only
        /// </summary>
        public void UploadAvatarFromPC()
        {
            FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png", ".jpeg"));
            FileBrowser.SetDefaultFilter("Images");
            FileBrowser.ShowLoadDialog(OnImageLoadFromPC, OnImageLoadFailed);
        }

    }
}



