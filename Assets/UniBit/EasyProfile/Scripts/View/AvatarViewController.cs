using UnityEngine;
using UnityEngine.UI;

namespace EasyProfile
{
    public class AvatarViewController : MonoBehaviour
    {
        // UI elements
        [SerializeField]
        private Image AvatarImage = default;
        [SerializeField]
        private RectTransform AvatarRect = default;
        // Image scale for user avatar
        [SerializeField]
        private float AvatarSize = default;
        // Image scale for default avatar
        [SerializeField]
        private float DefaultAvatarSize = default;

        // Current loaded user ID
        private string CurrentUserID;

        /// <summary>
        /// Init current view controller
        /// </summary>
        /// <param name="_id">User ID</param>
        public void LoadAvatar(string _id)
        {
            ClearData();
            CurrentUserID = _id;
            if (EasyProfileManager.Instance.PROFILE_CONTROLLER.PROFILE_IMAGE_LOADED)
            {
                Texture2D _savedTexture = EasyProfileManager.Instance.PROFILE_CONTROLLER.CACHED_AVATAR;
                AvatarImage.sprite = Sprite.Create(_savedTexture, new Rect(0.0f, 0.0f, _savedTexture.width, _savedTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                ResizeAvarar(AvatarSize);
            }
            else
            {
                GetProfileImage();
            }
            
        }

        /// <summary>
        /// On Disable method
        /// </summary>
        private void OnDisable()
        {
            AvatarImage.sprite = null;
        }

        /// <summary>
        /// Clear view controller data
        /// </summary>
        private void ClearData()
        {
            DisplayDefaultAvatar();
            CurrentUserID = string.Empty; ;
        }

        /// <summary>
        /// Load and display user profile image
        /// </summary>
        private void GetProfileImage()
        {
            RequestGetProfileImage _request = new RequestGetProfileImage();
            _request.Id = CurrentUserID;
            _request.Size = EasyProfileManager.Instance.PROFILE_SETTING.ProfileAvatarSize;
            EasyProfileManager.Instance.GetProfileImage(_request, OnProfileImageGetted);
        }

        /// <summary>
        /// Profile image getted method
        /// </summary>
        /// <param name="_callback"></param>
        public void OnProfileImageGetted(CallbackGetProfileImage _callback)
        {
            if (_callback.IsSuccess)
            {
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(_callback.ImageBytes);
                AvatarImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

                ResizeAvarar(AvatarSize);
            }
            else
            {
                DisplayDefaultAvatar();
            }
        }

        /// <summary>
        /// Display default avatar
        /// </summary>
        public void DisplayDefaultAvatar()
        {
            Texture2D _defaultAvatar = EasyProfileManager.Instance.PROFILE_SETTING.DefaultAvatarTexture;
            AvatarImage.sprite = Sprite.Create(_defaultAvatar, new Rect(0.0f, 0.0f, _defaultAvatar.width, _defaultAvatar.height), new Vector2(0.5f, 0.5f), 100.0f);
            ResizeAvarar(DefaultAvatarSize);
        }

        /// <summary>
        /// Resize user avatar
        /// </summary>
        /// <param name="_size">Scale value</param>
        private void ResizeAvarar(float _size)
        {
            float _bodyWidth = _size;
            float _bodyHeight = _size;
            float _imageWidth = (float)AvatarImage.sprite.texture.width;
            float _imageHeight = (float)AvatarImage.sprite.texture.height;
            float _ratio = _imageWidth / _imageHeight;
            if (_imageWidth > _imageHeight)
            {
                _ratio = _imageHeight / _imageWidth;
            }
            float _expectedHeight = _bodyWidth / _ratio;
            if (_imageWidth > _imageHeight)
            {
                AvatarRect.sizeDelta = new Vector2(_expectedHeight, _bodyHeight);
            }
            else
            {
                AvatarRect.sizeDelta = new Vector2(_bodyWidth, _expectedHeight);
            }
        }
    }
}
