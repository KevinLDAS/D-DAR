using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;

namespace EasyProfile
{
    public class ProfileController : MonoBehaviour
    {

        // firebase user
        private FirebaseUser FireUser;
        public FirebaseUser FIREBASE_USER
        {
            get
            {
                return FireUser;
            }
            set
            {
                FireUser = value;
            }
        }

        private string UserId;
        /// <summary>
        /// Get ID of the current authorized user.
        /// </summary>
        public string USER_ID
        {
            get
            {
                if (FIREBASE_USER == null)
                {
                    return UserId;
                }
                else
                {
                    return FIREBASE_USER.UserId;
                }
                
            }
            set
            {
                UserId = value;
            }
        }

        /// <summary>
        /// Get user object of the current authorized user.
        /// </summary>
        private User userData;
        public User CURRENT_USER_DATA
        {
            get
            {
                return userData;
            }
            set
            {
                userData = value;
            }
        }

        /// <summary>
        /// Last saved user avatar. Updated when call GetProfileImage()
        /// </summary>
        private Texture2D CachedTexture;
        public Texture2D CACHED_AVATAR
        {
            get
            {
                return CachedTexture;
            }
            set
            {
                CachedTexture = value;
                if (CachedTexture != null)
                {
                    IsProfileImageLoaded = true;
                }
            }
        }

        /// <summary>
        /// Checks if users image profile loaded
        /// </summary>
        private bool IsProfileImageLoaded;
        public bool PROFILE_IMAGE_LOADED
        {
            get
            {
                return IsProfileImageLoaded;
            }
            set
            {
                IsProfileImageLoaded = value;
            }
        }

        /// <summary>
        /// Checks if current id compare with current authorized user.
        /// </summary>
        /// <param name="_userId">Input user ID</param>
        /// <returns></returns>
        public bool IsMine(string _userId)
        {
            return _userId == USER_ID;
        }

        /// <summary>
        /// Checks if user is authorized
        /// </summary>
        /// <returns></returns>
        public bool IsLogined()
        {
            return FireUser != null;
        }

        /// <summary>
        /// Clear user data of the current authorized user.
        /// </summary>
        public void ClearUser()
        {
            FIREBASE_USER = null;
            PROFILE_IMAGE_LOADED = false;
            CURRENT_USER_DATA = null;
            CACHED_AVATAR = null;
        }

    }

    /// <summary>
    /// User object class
    /// </summary>
    public class User
    {
        public string UserID;
        public string FullName;
        public string FirstName;
        public string LastName;
        public string Phone;
        public string DataRegistration;
        public string LastActivity;
        public string NickName;

        public List<CustomValue> CustomValues = new List<CustomValue>();
    }
}