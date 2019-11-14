using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyProfile
{
    public class EasyProfileSettings : ScriptableObject
    {
        [Header("API Settings")]
        [Tooltip("if enabled - you will only use api plugin. Drag and drop the solution will be off and you will have to build all the logic yourself.")]
        public bool UseApiOnly;
        [Header("Registration")]
        [Tooltip("Min password characters count to pass registration.")]
        public int MinAllowPasswordCharacters;
        [Tooltip("The symbol to be contained in the login.")]
        public string EmailValidationCharacter;
        [Tooltip("If enabled, the user must activate his account in the mail before completing the registration.")]
        public bool UseEmailConfirm;
        [Header("Login")]
        [Tooltip("If enabled, user credentials data will be automatically saved after success login and the next input will not require a login and password, it will login automatically.")]
        public bool AutoSaveUserCredentials;
        [Tooltip("If enabled, user will login automatically on start scene init")]
        public bool AutoLoginOnStart;
        [Header("Date")]
        [Tooltip("Specifies the format in which the date will be displayed in the application.")]
        public string SystemDateFormat;
        [Header("Scenes")]
        [Tooltip("If enabled, the system will automatically go to the main scene after a successful login and also to the authorization scene after a successful logout. Turn off if you want to make your own logic of transition between scenes")]
        public bool AllowToLoadScene = true;
        [Tooltip("The name of the scene where the user will log in. Must be included in build settings.")]
        public string LoginSceneName;
        [Tooltip("The name of the scene where the user will go after successful authorization. Must be included in build settings.")]
        public string ProfileSceneName;
        [Header("Windows Prefab")]
        [Tooltip("Login prefab window.")]
        public GameObject LoginWindowPrefab;
        [Tooltip("Registration prefab window.")]
        public GameObject RegistrationWindowPrefab;
        [Tooltip("Loading prefab window.")]
        public GameObject LoadingWindowPrefab;
        [Tooltip("Popup prefab window.")]
        public GameObject PopupWindowPrefab;
        [Tooltip("User profile prefab window.")]
        public GameObject UserProfileWindowPrefab;
        [Tooltip("Restore password prefab window.")]
        public GameObject RestorePasswordWindowPrefab;
        [Header("Images")]
        [Range(1, 100)]
        [Tooltip("Image compression before upload.")]
        public int UploadImageQuality = 50;
        [Tooltip("Displayed avatar size.")]
        public ImageSize ProfileAvatarSize;
        [Tooltip("Texture of default avatar.")]
        public Texture2D DefaultAvatarTexture;
        [Header("Custom Data")]
        [Tooltip("Custom user data to be used in your project. To add new ones - expand the array and add your key values and type of variable.")]
        public List<CustomValue> CustomValues = new List<CustomValue>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Return custom value type by value id</returns>
        public CustomValueType GetValueTypeByKey(string _key)
        {
            CustomValueType _tempType = CustomValueType.String;
            foreach (CustomValue _data in CustomValues)
            {
                if (_data.ValueID == _key)
                {
                    _tempType = _data.ValueType;
                    break;
                }
            }
            return _tempType;
        }
    }
}



