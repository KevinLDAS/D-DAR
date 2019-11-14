using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace EasyProfile
{
    public class EasyProfileManager : MonoBehaviour
    {
        // Variables of all controllers
        private EasyProfileSettings ProfileSettings;
        private EasyProfileSolution ProfileSolution;
        private ProfileViewController ViewController;
        private ProfileController ProfileController;
        private ProfileFirebaseController FirebaseController;

        // An lists of all the methods that are needed to handle events
        private event Action<CallbackInitFirebaseMessage> OnInitActions;
        private event Action<CallbackLoginMessage> OnLoginActions;
        private event Action<CallbackRegistrationMessage> OnRegistrationActions;
        private event Action<CallbackSetUserDataMessage> OnSetUserDataActions;
        private event Action<CallbackGetUserDataMessage> OnGetUserDataActions;
        private event Action<CallBackUploadImage> OnUploadAvatarActions;
        private event Action<CallbackGetProfileImage> OnGetProfileImageActions;
        private event Action<CallbackLogOut> OnLogOutActions;
        private event Action<CallbackSetUserCustomValue> OnSetCustomDataActions;
        private event Action<CallbackGetUserCustomValue> OnGetCustomDataActions;

        // flag for manager initialization
        private static bool IsInited = false;

        private static EasyProfileManager instance;
        /// <summary>
        /// Initializes and returns the manager Instance. 
        /// </summary>
        public static EasyProfileManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<EasyProfileManager>();
                }
                return instance;
            }

        }

        /// <summary>
        /// Provides access to View Controller. The controller is responsible for the visual part of the plugin.
        /// </summary>
        public ProfileViewController VIEW_CONTROLLER
        {
            get
            {
                return ViewController;
            }
        }

        /// <summary>
        /// Provides access to User Profile Controller. This controller store all user data.
        /// </summary>
        public ProfileController PROFILE_CONTROLLER
        {
            get
            {
                if (!FIREBASE_CONTROLLER.IsUserLoggined())
                {
                    Debug.LogError("To access PROFILE CONTROLLER - you must be logged first");
                }
                return ProfileController;
            }
        }

        /// <summary>
        /// Provides access to Firebase Controller. This controller is responsible for all requests to Firebase Database or Storage.
        /// </summary>
        public ProfileFirebaseController FIREBASE_CONTROLLER
        {
            get
            {
                if (!FirebaseController.IsFirebaseInited())
                {
                    Debug.LogError("To access FIREBASE CONTROLLER - you must be inited first. Call init method");
                }
                return FirebaseController;
            }
        }

        /// <summary>
        /// Provides access to plugin settings.
        /// </summary>
        public EasyProfileSettings PROFILE_SETTING
        {
            get
            {
                return ProfileSettings;
            }
        }

        /// <summary>
        /// Returns solution controller. This manager is responsible for drag and drop the asset decision.
        /// </summary>
        public EasyProfileSolution PROFILE_SOLUTION
        {
            get
            {
                return ProfileSolution;
            }
        }

        /// <summary>
        /// Start point of manager
        /// </summary>
        private void Awake()
        {
            Init();
        }

        /// <summary>
        /// Initialization EasyProfileManager
        /// </summary>
        private void Init()
        {
            if (instance != null && IsInited)
            {
                //Remove dublicate
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
            LoadSettings();
            LoadControllers();
            IsInited = true;
        }

        /// <summary>
        /// Loads aseets setting
        /// </summary>
        private void LoadSettings()
        {
            ProfileSettings = Resources.Load<EasyProfileSettings>("Scriptable/EasyProfileSettings");
        }

        /// <summary>
        /// Loads all manager controllers 
        /// </summary>
        private void LoadControllers()
        {
            ViewController = gameObject.GetComponentInChildren<ProfileViewController>();
            ProfileController = gameObject.GetComponentInChildren<ProfileController>();
            FirebaseController = gameObject.GetComponentInChildren<ProfileFirebaseController>();
            ProfileSolution = gameObject.GetComponentInChildren<EasyProfileSolution>();
        }

        /// <summary>
        /// Returns the local time and date on the device in a specific format. The format can be set in the asset settings (EasySystems->EasyProfile->Settings)
        /// </summary>
        /// <returns></returns>
        public string GetSystemDate()
        {
            return DateTime.Now.ToString(ProfileSettings.SystemDateFormat);
        }

        /// <summary>
        /// Saves user credentials
        /// </summary>
        /// <param name="_mail">User mail</param>
        /// <param name="_password">User password</param>
        public void SaveUserCredentials(string _mail, string _password)
        {
            PlayerPrefs.SetString(Constants.LoginSaveKey, _mail);
            PlayerPrefs.SetString(Constants.PasswordSaveKey, _password);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Checks if user credentials are saved
        /// </summary>
        /// <returns></returns>
        public bool HasSavedCredentials()
        {
            return PlayerPrefs.HasKey(Constants.LoginSaveKey) && PlayerPrefs.HasKey(Constants.PasswordSaveKey);
        }

        /// <summary>
        /// Clears last saved user credentials
        /// </summary>
        public void ClearUserCredentials()
        {
            PlayerPrefs.DeleteKey(Constants.LoginSaveKey);
            PlayerPrefs.DeleteKey(Constants.PasswordSaveKey);
        }

        /// <summary>
        /// Returns last saved user credentials
        /// </summary>
        /// <returns></returns>
        public UserCredentials GetSavedCredentials()
        {
            if (HasSavedCredentials())
            {
                UserCredentials _credentials = new UserCredentials();
                _credentials.UserLogin = PlayerPrefs.GetString(Constants.LoginSaveKey);
                _credentials.UserPassword = PlayerPrefs.GetString(Constants.PasswordSaveKey);
                return _credentials;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Initialization Firebase SDK
        /// </summary>
        /// <param name="_callback">Add initialization completion method</param>
        public void InitFirebase(Action<CallbackInitFirebaseMessage> _callback = null)
        {
            FirebaseController.InitFirebase(_callback);
        }

        /// <summary>
        /// Returns true if Firebase SDK is already initialized
        /// </summary>
        /// <returns></returns>
        public bool IsFirebaseInited()
        {
            if (FirebaseController == null)
            {
                FirebaseController = gameObject.GetComponentInChildren<ProfileFirebaseController>();
            }
            return FirebaseController.IsFirebaseInited();
        }

        /// <summary>
        /// Check if user is logined
        /// </summary>
        /// <returns></returns>
        public bool IsUserLogined()
        {
            return FirebaseController.IsUserLoggined();
        }

        /// <summary>
        /// User registration. Use users email and password to register new user
        /// </summary>
        /// <param name="_email">User email</param>
        /// <param name="_password">User email</param>
        /// <param name="_callback">Add completion method</param>
        public void RegisterNewUser(string _email, string _password, Action<CallbackRegistrationMessage> _callback)
        {
            FIREBASE_CONTROLLER.RegisterNewUser(_email, _password, _callback);
        }

        /// <summary>
        /// User registration. Use users credentional to register new user
        /// </summary>
        /// <param name="_credentials">User credentional object</param>
        /// <param name="_callback">Add completion method</param>
        public void RegisterNewUser(UserCredentials _credentials, Action<CallbackRegistrationMessage> _callback)
        {
            FIREBASE_CONTROLLER.RegisterNewUser(_credentials.UserLogin, _credentials.UserPassword, _callback);
        }

        /// <summary>
        /// User registration. Use users custom token id to register new user
        /// </summary>
        /// <param name="_tokenID">Your custom token ID</param>
        /// <param name="_user">User data</param>
        /// <param name="_callback">Add completion method</param>
        public void RegisterNewUserWithCustomTokenID(string _tokenID, User _user, Action<CallbackRegistrationMessage> _callback)
        {
            FIREBASE_CONTROLLER.RegisterNewUserWithCustomTokenID(_tokenID, _user, _callback);
        }

        /// <summary>
        /// User registration. Use custom auth system
        /// </summary>
        /// <param name="_userID">Custom User ID</param>
        /// <param name="_user">User data</param>
        /// <param name="_callback">Add completion method</param>
        public void RegisterNewUserWithCustomAuthSystem(string _userID, User _user, Action<CallbackRegistrationMessage> _callback)
        {
            FIREBASE_CONTROLLER.RegisterNewUserWithCustomAuthSystem(_userID, _user, _callback);
        }

        /// <summary>
        /// User authorization. Use users email and password to sign in
        /// </summary>
        /// <param name="_login">Users email</param>
        /// <param name="_password">Users password</param>
        /// <param name="_callback">Add sign in completion method</param>
        public void LogIn(string _login, string _password, Action<CallbackLoginMessage> _callback = null)
        {
            FIREBASE_CONTROLLER.LogIn(_login, _password, _callback);
        }

        /// <summary>
        /// User authorization. Use users credentials to sign in
        /// </summary>
        /// <param name="_credentials"></param>
        /// <param name="_callback"></param>
        public void LogIn(UserCredentials _credentials, Action<CallbackLoginMessage> _callback = null)
        {
            FIREBASE_CONTROLLER.LogIn(_credentials.UserLogin, _credentials.UserPassword, _callback);
        }

        /// <summary>
        /// User authorization. Use custom tokenID to sign in
        /// </summary>
        /// <param name="_tokenID">Custom token ID</param>
        /// <param name="_callback">Add sign in completion method</param>
        public void LogInWithCustomTokenID(string _tokenID, Action<CallbackLoginMessage> _callback = null)
        {
            FIREBASE_CONTROLLER.LogInWithCustomTokenID(_tokenID, _callback);
        }

        /// <summary>
        /// User authorization. Use custom auth system to sign in
        /// </summary>
        /// <param name="_userID">Custom user ID</param>
        /// <param name="_callback">Add sign in completion method</param>
        public void LogInWithCustomAuthSystem(string _userID, Action<CallbackLoginMessage> _callback = null)
        {
            FIREBASE_CONTROLLER.LogInWithCustomAuthSystem(_userID, _callback);
        }


        /// <summary>
        /// Upload user avatar image to Firebase Storage. 
        /// </summary>
        /// <param name="_request">Request object. Use RequestUploadImage _request = new RequestUploadImage()</param>
        /// <param name="_callback">Add upload completion method</param>
        public void UploadAvatar(RequestUploadImage _request, Action<CallBackUploadImage> _callback = null)
        {
            FIREBASE_CONTROLLER.UploadAvatar(_request, _callback);
        }

        /// <summary>
        /// Get user avatar image from Firebase Storage.
        /// </summary>
        /// <param name="_request">Request object. Use RequestGetProfileImage _request = new RequestGetProfileImage()</param>
        /// <param name="_callback">Add completion method</param>
        public void GetProfileImage(RequestGetProfileImage _request, Action<CallbackGetProfileImage> _callback = null)
        {
            FIREBASE_CONTROLLER.GetProfileImage(_request, _callback);
        }

        /// <summary>
        /// Get user data by user id.
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserDataByID(string _userID, Action<CallbackGetUserDataMessage> _callback)
        {
            FIREBASE_CONTROLLER.GetUserData(_userID, _callback);
        }

        /// <summary>
        /// Sends an account activation letter to the user in the mail. Use this method after successful user registration.
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void SendMailVerification(Action<CallbackSendVerificationEmail> _callback = null)
        {
            FIREBASE_CONTROLLER.SendVerifcationEmail(_callback);
        }

        /// <summary>
        /// Sends a password reset email to the user.
        /// </summary>
        /// <param name="_mail">User email</param>
        /// <param name="_callback">Add completion method</param>
        public void SendResetPasswordEmail(string _mail, Action<CallbackSendResetPasswordEmail> _callback = null)
        {
            FIREBASE_CONTROLLER.SendResetPasswordEmail(_mail, _callback);
        }

        /// <summary>
        /// Create or set user data
        /// </summary>
        /// <param name="_user">User object</param>
        /// <param name="_callback">Add completion method</param>
        public void SetUserData(User _user, Action<CallbackSetUserDataMessage> _callback = null)
        {
            FIREBASE_CONTROLLER.SetUserData(_user, _callback);
        }

        /// <summary>
        /// Get user data of the current authorized user.
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void GetUserData( Action<CallbackGetUserDataMessage> _callback)
        {
            FIREBASE_CONTROLLER.GetUserData(PROFILE_CONTROLLER.USER_ID, _callback);
        }

        /// <summary>
        /// Get the full name of the current authorized user.
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void GetUserFullName(Action<string> _callback)
        {
            FIREBASE_CONTROLLER.GetUserFullName(PROFILE_CONTROLLER.USER_ID, _callback);
        }

        /// <summary>
        /// Get the full name of user by id.
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserFullNameByID(string _userID, Action<string> _callback)
        {
            FIREBASE_CONTROLLER.GetUserFullName(_userID, _callback);
        }

        /// <summary>
        /// Set user full name of the current authorized user.
        /// </summary>
        /// <param name="_value">Full name value</param>
        /// <param name="_callback">Add completion method</param>
        public void SetUserFullName(string _value, Action<CallbackSetUserValue> _callback = null)
        {
            FIREBASE_CONTROLLER.SetUserFullName(PROFILE_CONTROLLER.USER_ID, _value, _callback);
        }

        /// <summary>
        /// Get the last name of the current authorized user.
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void GetUserLastName(Action<string> _callback)
        {
            FIREBASE_CONTROLLER.GetUserLastName(PROFILE_CONTROLLER.USER_ID, _callback);
        }

        /// <summary>
        /// Get the last name of user by id.
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserLastNameByID(string _userID, Action<string> _callback)
        {
            FIREBASE_CONTROLLER.GetUserLastName(_userID, _callback);
        }

        /// <summary>
        /// Set user last name of the current authorized user.
        /// </summary>
        /// <param name="_value">Last name value</param>
        /// <param name="_callback">Add completion method</param>
        public void SetUserLastName(string _value, Action<CallbackSetUserValue> _callback = null)
        {
            FIREBASE_CONTROLLER.SetUserLastName(PROFILE_CONTROLLER.USER_ID, _value, _callback);
        }

        /// <summary>
        /// Get the first name of the current authorized user.
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void GetUserFirstName(Action<string> _callback)
        {
            FIREBASE_CONTROLLER.GetUserFirstName(PROFILE_CONTROLLER.USER_ID, _callback);
        }

        /// <summary>
        /// Get the first name of user by id.
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserFirstNameByID(string _userID, Action<string> _callback)
        {
            FIREBASE_CONTROLLER.GetUserFirstName(_userID, _callback);
        }

        /// <summary>
        /// Set user first name of the current authorized user.
        /// </summary>
        /// <param name="_value">First name value</param>
        /// <param name="_callback">Add completion method</param>
        public void SetUserFirstName(string _value, Action<CallbackSetUserValue> _callback = null)
        {
            FIREBASE_CONTROLLER.SetUserFirstName(PROFILE_CONTROLLER.USER_ID, _value, _callback);
        }

        /// <summary>
        /// Get the nick name of the current authorized user.
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void GetUserNickName(Action<string> _callback)
        {
            FIREBASE_CONTROLLER.GetUserNickName(PROFILE_CONTROLLER.USER_ID, _callback);
        }

        /// <summary>
        /// Get the nick name of user by id.
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserNickNameByID(string _userID, Action<string> _callback)
        {
            FIREBASE_CONTROLLER.GetUserNickName(_userID, _callback);
        }

        /// <summary>
        /// Set user nick name of the current authorized user.
        /// </summary>
        /// <param name="_value">Nick name value</param>
        /// <param name="_callback">Add completion method</param>
        public void SetUserNickName(string _value, Action<CallbackSetUserValue> _callback = null)
        {
            FIREBASE_CONTROLLER.SetUserNickName(PROFILE_CONTROLLER.USER_ID, _value, _callback);
        }

        /// <summary>
        /// Get user registration date of the current authorized user.
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void GetUserRegistrationDate(Action<string> _callback)
        {
            FIREBASE_CONTROLLER.GetUserRegistrationDate(PROFILE_CONTROLLER.USER_ID, _callback);
        }

        /// <summary>
        /// Get user registration date by user ID
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserRegistrationDateByID(string _userID, Action<string> _callback)
        {
            FIREBASE_CONTROLLER.GetUserRegistrationDate(_userID, _callback);
        }

        /// <summary>
        /// Get user phone number of the current authorized user.
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void GetUserPhone(Action<string> _callback)
        {
            FIREBASE_CONTROLLER.GetUserPhone(PROFILE_CONTROLLER.USER_ID, _callback);
        }

        /// <summary>
        /// Get user phone number date by user ID
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserPhoneByID(string _userID, Action<string> _callback)
        {
            FIREBASE_CONTROLLER.GetUserPhone(_userID, _callback);
        }

        /// <summary>
        /// Set user phone number of the current authorized user.
        /// </summary>
        /// <param name="_value">Phone number value</param>
        /// <param name="_callback">Add completion method</param>
        public void SetUserPhone(string _value, Action<CallbackSetUserValue> _callback = null)
        {
            FIREBASE_CONTROLLER.SetUserPhone(PROFILE_CONTROLLER.USER_ID, _value, _callback);
        }

        /// <summary>
        /// Get all user of Firebase Database
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void GetAllUsers(Action<CallbackGetAllUsers> _callback)
        {
            FIREBASE_CONTROLLER.GetAllUsers(_callback);
        }


        /// <summary>
        /// Call to log out of the current authorized user.
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void LogOut(Action<CallbackLogOut> _callback = null)
        {
            FIREBASE_CONTROLLER.LogOut(_callback);
        }

        /// <summary>
        /// Search users by string value. You can search for users by name, nickname or phone number.
        /// </summary>
        /// <param name="_searchValue">Search Value</param>
        /// <param name="_callback">Add completion method</param>
        public void SearchUsers(string _searchValue, Action<CallbackSearchUsers> _callback)
        {
            FIREBASE_CONTROLLER.SearchUsers(_searchValue, _callback);
        }

        /// <summary>
        /// Set user custom value by user ID. All custom values must be configured in plugin settings (EasySystems->EasyProfile->Settings)
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_valueID">Custom value id</param>
        /// <param name="_value">Custom value</param>
        /// <param name="_callback">Add completion method</param>
        public void SetCustomValueByUserID(string _userID, string _valueID, object _value, Action<CallbackSetUserCustomValue> _callback = null)
        {
            FIREBASE_CONTROLLER.SetUserCustomValue(_userID, _valueID, _value, _callback);
        }

        /// <summary>
        /// Set user custom value of the current authorized user. All custom values must be configured in plugin settings (EasySystems->EasyProfile->Settings)
        /// </summary>
        /// <param name="_valueID">Custom value id</param>
        /// <param name="_value">Custom value</param>
        /// <param name="_callback">Add completion method</param>
        public void SetCustomValue(string _valueID, object _value, Action<CallbackSetUserCustomValue> _callback = null)
        {
            FIREBASE_CONTROLLER.SetUserCustomValue(PROFILE_CONTROLLER.USER_ID, _valueID, _value, _callback);
        }

        /// <summary>
        /// Get user custom value of the current authorized user. All custom values must be configured in plugin settings (EasySystems->EasyProfile->Settings)
        /// </summary>
        /// <param name="_valueID">Value ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetCustomValue(string _valueID, Action<CallbackGetUserCustomValue> _callback)
        {
            FIREBASE_CONTROLLER.GetUserCustomValue(PROFILE_CONTROLLER.USER_ID, _valueID, _callback);
        }

        /// <summary>
        /// Get user custom value by user ID. All custom values must be configured in plugin settings (EasySystems->EasyProfile->Settings)
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_valueID">Value ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetCustomValueByUserID(string _userID, string _valueID, Action<CallbackGetUserCustomValue> _callback)
        {
            FIREBASE_CONTROLLER.GetUserCustomValue(_userID, _valueID, _callback);
        }

        /// <summary>
        /// Update user last activity value
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void UpdateUserActivity(Action<CallbackSetUserActivity> _callback = null)
        {
            FIREBASE_CONTROLLER.UpdateUserActivity(_callback);
        }

        /// <summary>
        /// Get user last activity by user id.
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserLastActivity(string _userID, Action<CallbackGetUserActivity> _callback)
        {
            FIREBASE_CONTROLLER.GetUserLastActivity(_userID, _callback);
        }

        /// <summary>
        /// Get current server time. Availbale when user is logined
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void GetServerTimestamp(Action<CallbackGetServerTimestamp> _callback)
        {
            FIREBASE_CONTROLLER.GetServerTimestamp(_callback);
        }

        /// <summary>
        /// Get download url of user avatar of the current authorized user
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_size">Avatar size</param>
        /// <param name="_callback">Add completion method</param>
        public void GetProfileImageURL(ImageSize _size, Action<CallbackGetProfileImageURL> _callback)
        {
            FIREBASE_CONTROLLER.GetProfileImageURL(PROFILE_CONTROLLER.USER_ID, _size, _callback);
        }

        /// <summary>
        /// Get download url of user avatar of the current authorized user
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_size">Avatar size</param>
        /// <param name="_callback">Add completion method</param>
        public void GetProfileImageURLByUserID(string _userID, ImageSize _size, Action<CallbackGetProfileImageURL> _callback)
        {
            FIREBASE_CONTROLLER.GetProfileImageURL(_userID, _size, _callback);
        }

        // Event Subscribers

        /// <summary>
        /// Subscribe to Firebase initialization complete event
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void AddOnInitAction(Action<CallbackInitFirebaseMessage> _callback)
        {
            OnInitActions += _callback;
        }

        /// <summary>
        /// Remove subscribe to Firebase initialization event
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void RemoveOnInitAction(Action<CallbackInitFirebaseMessage> _callback)
        {
            OnInitActions -= _callback;
        }

        /// <summary>
        /// Raises a Firebase initialization event
        /// </summary>
        /// <param name="_callback">Callback response object</param>
        public void RaiseInitActions(CallbackInitFirebaseMessage _callback)
        {
            OnInitActions?.Invoke(_callback);
        }

        /// <summary>
        /// Subscribe to log in event
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void AddOnLoginAction(Action<CallbackLoginMessage> _callback)
        {
            OnLoginActions += _callback;
        }

        /// <summary>
        /// Remove subscribe to log in event
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void RemoveOnLoginAction(Action<CallbackLoginMessage> _callback)
        {
            OnLoginActions -= _callback;
        }

        /// <summary>
        /// Raises a log in event
        /// </summary>
        /// <param name="_callbak">Callback response object</param>
        public void RaiseLoginActions(CallbackLoginMessage _callbak)
        {
            OnLoginActions?.Invoke(_callbak);
        }

        /// <summary>
        /// Subscribe to registration complete event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void AddOnRegistrationAction(Action<CallbackRegistrationMessage> _callback)
        {
            OnRegistrationActions += _callback;
        }

        /// <summary>
        /// Remove subscribe to registration event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void RemoveOnRegistrationAction(Action<CallbackRegistrationMessage> _callback)
        {
            OnRegistrationActions -= _callback;
        }

        /// <summary>
        /// Raises a registration event
        /// </summary>
        /// <param name="_callbak">Callback response object</param>
        public void RaiseRegistrationActions(CallbackRegistrationMessage _callbak)
        {
            OnRegistrationActions?.Invoke(_callbak);
        }

        /// <summary>
        /// Subscribe to set user data complete event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void AddOnSetUserDataCompleteAction(Action<CallbackSetUserDataMessage> _callback)
        {
            OnSetUserDataActions += _callback;
        }

        /// <summary>
        /// Remove subscribe to set user data complete event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void RemoveOnSetUserDataCompleteAction(Action<CallbackSetUserDataMessage> _callback)
        {
            OnSetUserDataActions -= _callback;
        }

        /// <summary>
        /// Raises a set user data complete event
        /// </summary>
        /// <param name="_callbak">Callback response object</param>
        public void RaiseSetUserDataActions(CallbackSetUserDataMessage _callbak)
        {
            OnSetUserDataActions?.Invoke(_callbak);
        }

        /// <summary>
        /// Subscribe to get user data complete event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void AddOnGetUserDataCompleteAction(Action<CallbackGetUserDataMessage> _callback)
        {
            OnGetUserDataActions += _callback;
        }

        /// <summary>
        /// Remove subscribe to get user data complete event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void RemoveOnGetUserDataCompleteAction(Action<CallbackGetUserDataMessage> _callback)
        {
            OnGetUserDataActions -= _callback;
        }

        /// <summary>
        /// Raises a get user data complete event
        /// </summary>
        /// <param name="_callbak">Callback response object</param>
        public void RaiseGetUserDataActions(CallbackGetUserDataMessage _callbak)
        {
            OnGetUserDataActions?.Invoke(_callbak);
        }

        /// <summary>
        /// Subscribe to upload proflie image complete event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void AddOnUploadAvatarCompleteAction(Action<CallBackUploadImage> _callback)
        {
            OnUploadAvatarActions += _callback;
        }

        /// <summary>
        /// Remoce subscribe to upload proflie image complete event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void RemoveOnUploadCompleteAction(Action<CallBackUploadImage> _callback)
        {
            OnUploadAvatarActions -= _callback;
        }

        /// <summary>
        /// Raises a upload profile image complete event
        /// </summary>
        /// <param name="_callbak">Callback response object</param>
        public void RaiseUploadAvatarActions(CallBackUploadImage _callbak)
        {
            OnUploadAvatarActions?.Invoke(_callbak);
        }

        /// <summary>
        /// Subscribe to get proflie image complete event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void AddOnGetProfileImageAction(Action<CallbackGetProfileImage> _callback)
        {
            OnGetProfileImageActions += _callback;
        }

        /// <summary>
        /// Remove subscribe to get proflie image complete event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void RemoveOnGetProfileImageAction(Action<CallbackGetProfileImage> _callback)
        {
            OnGetProfileImageActions -= _callback;
        }

        /// <summary>
        /// Raises a get profile image complete event
        /// </summary>
        /// <param name="_callbak">Callback response object</param>
        public void RaiseGetProfileImageActions(CallbackGetProfileImage _callbak)
        {
            OnGetProfileImageActions?.Invoke(_callbak);
        }

        /// <summary>
        /// Subscribe to log out event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void AddOnLogOutAction(Action<CallbackLogOut> _callback)
        {
            OnLogOutActions += _callback;
        }

        /// <summary>
        /// Remove subscribe to log out event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void RemoveOnLogOutAction(Action<CallbackLogOut> _callback)
        {
            OnLogOutActions -= _callback;
        }

        /// <summary>
        /// Raises a log out event
        /// </summary>
        /// <param name="_callbak">Callback response object</param>
        public void RaiseLogOutActions(CallbackLogOut _callbak)
        {
            OnLogOutActions?.Invoke(_callbak);
        }

        /// <summary>
        /// Subscribe to set cusmtom data complete event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void AddOnSetCustomDataAction(Action<CallbackSetUserCustomValue> _callback)
        {
            OnSetCustomDataActions += _callback;
        }

        /// <summary>
        /// Remove subscribe to set cusmtom data complete event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void RemoveOnSetCustomDataAction(Action<CallbackSetUserCustomValue> _callback)
        {
            OnSetCustomDataActions -= _callback;
        }

        /// <summary>
        /// Raises a set custom data event
        /// </summary>
        /// <param name="_callbak">Callback response object</param>
        public void RaiseOnSetCustomDataActions(CallbackSetUserCustomValue _callbak)
        {
            OnSetCustomDataActions?.Invoke(_callbak);
        }

        /// <summary>
        /// Subscribe to get cusmtom data complete event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void AddOnGetCustomDataAction(Action<CallbackGetUserCustomValue> _callback)
        {
            OnGetCustomDataActions += _callback;
        }

        /// <summary>
        /// Remove subscribe to get cusmtom data complete event.
        /// </summary>
        /// <param name="_callback">Subscribe method</param>
        public void RemoveOnGetCustomDataAction(Action<CallbackGetUserCustomValue> _callback)
        {
            OnGetCustomDataActions -= _callback;
        }

        /// <summary>
        /// Raises a get custom data event
        /// </summary>
        /// <param name="_callbak">Callback response object</param>
        public void RaiseOnGetCustomDataActions(CallbackGetUserCustomValue _callbak)
        {
            OnGetCustomDataActions?.Invoke(_callbak);
        }

        
    }

    /// <summary>
    /// User credential class
    /// </summary>
    public class UserCredentials
    {
        public string UserLogin;
        public string UserPassword;
    }


}

