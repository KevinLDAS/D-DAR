using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using Firebase;
using Firebase.Extensions;


namespace EasyProfile
{
    public class ProfileFirebaseController : MonoBehaviour
    {
        // Default Firebase Auth Instance;
        private FirebaseAuth Auth;
        // Default Firebase Database Instance;
        private FirebaseDatabase Database;
        // Default Firebase Storage Instance;
        private FirebaseStorage Storage;
        // Return true if Firebase is already inited
        private bool FirebaseIsInited = false;
        // Return true if user is already loggined
        private bool UserIsLoggined = false;

        /// <summary>
        /// Clean task from memory
        /// </summary>
        /// <param name="_task">Task to clean</param>
        private void CleanTask(Task _task)
        {
            _task.Dispose();
            _task = null;
        }

        /// <summary>
        /// Check if Firebase is inited
        /// </summary>
        /// <returns>Return true if Firebase is already inited</returns>
        public bool IsFirebaseInited()
        {
            return FirebaseIsInited;
        }

        /// <summary>
        /// Check if user is loggined
        /// </summary>
        /// <returns>Return true if user is already loggined</returns>
        public bool IsUserLoggined()
        {
            return UserIsLoggined;
        }


        /// <summary>
        /// Initialization Firebase SDK
        /// </summary>
        /// <param name="_callback">Add initialization completion method</param>
        public void InitFirebase(Action<CallbackInitFirebaseMessage> _callback = null)
        {
            CallbackInitFirebaseMessage _response = new CallbackInitFirebaseMessage();
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _response.IsSuccess = true;
                    Auth = FirebaseAuth.DefaultInstance;
                    Database = FirebaseDatabase.DefaultInstance;
                    Storage = FirebaseStorage.DefaultInstance;
                    FirebaseIsInited = true;
                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
                EasyProfileManager.Instance.RaiseInitActions(_response);
                if (_callback != null)
                {
                    _callback.Invoke(_response);
                }
            });
        }

        /// <summary>
        /// User authorization. Use users email and password to sign in
        /// </summary>
        /// <param name="_login">Users email</param>
        /// <param name="_password">Users password</param>
        /// <param name="_callback">Add sign in completion method</param> 
        public void LogIn(string _login, string _password, Action<CallbackLoginMessage> _callback = null)
        {
            CallbackLoginMessage _logMsg = new CallbackLoginMessage();
            UserCredentials _userCredentials = new UserCredentials();
            _userCredentials.UserLogin = _login;
            _userCredentials.UserPassword = _password;
            Auth.SignInWithEmailAndPasswordAsync(_login, _password).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    _logMsg.IsSuccess = false;
                    _logMsg.ErrorMessage = "SignInWithEmailAndPasswordAsync was canceled.";
                    if (_callback != null)
                    {
                        _callback.Invoke(_logMsg);
                    }
                    CleanTask(task);
                    return;
                }
                if (task.IsFaulted)
                {
                    _logMsg.IsSuccess = false;
                    _logMsg.ErrorMessage = task.Exception.Message;
                    if (_callback != null)
                    {
                        _callback.Invoke(_logMsg);
                    }
                    CleanTask(task);
                    return;
                }
                
                FirebaseUser newUser = task.Result;
                if (EasyProfileManager.Instance.PROFILE_SETTING.UseEmailConfirm)
                {
                    if (newUser.IsEmailVerified)
                    {
                        UserIsLoggined = true;
                        _logMsg.IsSuccess = true;
                        _logMsg.UserID = newUser.UserId;
                        _logMsg.FUser = newUser;
                        _logMsg.Gredentials = _userCredentials;
                        EasyProfileManager.Instance.PROFILE_CONTROLLER.FIREBASE_USER = newUser;
                    }
                    else
                    {
                        _logMsg.MessageCode = ProfileMessageCode.EmailConfirm;
                        LogOut();
                    }
                    if (_callback != null)
                    {
                        _callback.Invoke(_logMsg);
                    }
                    CleanTask(task);
                }
                else
                {
                    UserIsLoggined = true;
                    _logMsg.IsSuccess = true;
                    _logMsg.UserID = newUser.UserId;
                    _logMsg.FUser = newUser;
                    _logMsg.Gredentials = _userCredentials;
                    EasyProfileManager.Instance.PROFILE_CONTROLLER.FIREBASE_USER = newUser;
                    if (_callback != null)
                    {
                        _callback.Invoke(_logMsg);
                    }
                    CleanTask(task);
                }
                EasyProfileManager.Instance.RaiseLoginActions(_logMsg);
            });
        }

        /// <summary>
        /// User authorization. Use custom tokenID to sign in
        /// </summary>
        /// <param name="_tokenID">Custom token ID</param>
        /// <param name="_callback">Add sign in completion method</param>
        public void LogInWithCustomTokenID(string _tokenID, Action<CallbackLoginMessage> _callback = null)
        {
            CallbackLoginMessage _logMsg = new CallbackLoginMessage();
            Auth.SignInWithCustomTokenAsync(_tokenID).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    _logMsg.IsSuccess = false;
                    _logMsg.ErrorMessage = "SignInWithCustomTokenAsync was canceled.";
                    if (_callback != null)
                    {
                        _callback.Invoke(_logMsg);
                    }
                    CleanTask(task);
                    return;
                }
                if (task.IsFaulted)
                {
                    _logMsg.IsSuccess = false;
                    _logMsg.ErrorMessage = task.Exception.Message;
                    if (_callback != null)
                    {
                        _callback.Invoke(_logMsg);
                    }
                    CleanTask(task);
                    return;
                }

                FirebaseUser newUser = task.Result;
                UserIsLoggined = true;
                _logMsg.IsSuccess = true;
                _logMsg.UserID = newUser.UserId;
                _logMsg.FUser = newUser;
                EasyProfileManager.Instance.PROFILE_CONTROLLER.FIREBASE_USER = newUser;
                if (_callback != null)
                {
                    _callback.Invoke(_logMsg);
                }
                CleanTask(task);
                EasyProfileManager.Instance.RaiseLoginActions(_logMsg);
            });
        }

        /// <summary>
        /// User authorization. Use custom auth system to sign in
        /// </summary>
        /// <param name="_userID">Custom user ID</param>
        /// <param name="_callback">Add sign in completion method</param>
        public void LogInWithCustomAuthSystem(string _userID, Action<CallbackLoginMessage> _callback = null)
        {
            CallbackLoginMessage _logMsg = new CallbackLoginMessage();
            Auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    _logMsg.IsSuccess = false;
                    _logMsg.ErrorMessage = "SignInAnonymouslyAsync was canceled.";
                    if (_callback != null)
                    {
                        _callback.Invoke(_logMsg);
                    }
                    CleanTask(task);
                    return;
                }
                if (task.IsFaulted)
                {
                    _logMsg.IsSuccess = false;
                    _logMsg.ErrorMessage = task.Exception.Message;
                    if (_callback != null)
                    {
                        _callback.Invoke(_logMsg);
                    }
                    CleanTask(task);
                    return;
                }

                FirebaseUser newUser = task.Result;
                UserIsLoggined = true;
                _logMsg.IsSuccess = true;
                _logMsg.UserID = _userID;
                _logMsg.FUser = newUser;
                EasyProfileManager.Instance.PROFILE_CONTROLLER.USER_ID = _userID;
                if (_callback != null)
                {
                    _callback.Invoke(_logMsg);
                }
                CleanTask(task);
                EasyProfileManager.Instance.RaiseLoginActions(_logMsg);
            });
        }


        /// <summary>
        /// Log out from Firebase
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void LogOut(Action<CallbackLogOut> _callback = null)
        {
            CallbackLogOut _response = new CallbackLogOut();
            _response.IsSuccess = true;
            EasyProfileManager.Instance.RaiseLogOutActions(_response);
            if (_callback != null)
            {
                _callback.Invoke(_response);
            }
            FirebaseAuth.DefaultInstance.SignOut();
            EasyProfileManager.Instance.PROFILE_CONTROLLER.ClearUser();
            UserIsLoggined = false;
        }

        /// <summary>
        /// User registration. Use users email and password to register new user
        /// </summary>
        /// <param name="_email">User email</param>
        /// <param name="_password">User email</param>
        /// <param name="_callback">Add completion method</param>
        public void RegisterNewUser(string _email, string _password, Action<CallbackRegistrationMessage> _callback)
        {
            CallbackRegistrationMessage _regMsg = new CallbackRegistrationMessage();
            Auth.CreateUserWithEmailAndPasswordAsync(_email, _password).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    _regMsg.ErrorMessage = "CreateUserWithEmailAndPasswordAsync was canceled";
                    _callback.Invoke(_regMsg);
                    CleanTask(task);
                    return;
                }
                if (task.IsFaulted)
                {
                    _regMsg.ErrorMessage = "CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception;
                    _callback.Invoke(_regMsg);
                    CleanTask(task);
                    return;
                }
                FirebaseUser newUser = task.Result;
                _regMsg.UserID = newUser.UserId;
                _regMsg.IsComplete = true;
                EasyProfileManager.Instance.RaiseRegistrationActions(_regMsg);
                _callback.Invoke(_regMsg);
                CleanTask(task);
            });
        }

        /// <summary>
        /// User registration. Use users custom tiken id to register new user
        /// </summary>
        /// <param name="_tokenID">Your custom token ID</param>
        /// <param name="_user">User data</param>
        /// <param name="_callback">Add completion method</param>
        public void RegisterNewUserWithCustomTokenID(string _tokenID, User _user, Action<CallbackRegistrationMessage> _callback)
        {
            CallbackRegistrationMessage _regMsg = new CallbackRegistrationMessage();
            Auth.SignInWithCustomTokenAsync(_tokenID).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    _regMsg.ErrorMessage = "CreateUserWithEmailAndPasswordAsync was canceled";
                    _callback.Invoke(_regMsg);
                    CleanTask(task);
                    return;
                }
                if (task.IsFaulted)
                {
                    _regMsg.ErrorMessage = "CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception;
                    _callback.Invoke(_regMsg);
                    CleanTask(task);
                    return;
                }
                FirebaseUser newUser = task.Result;
                _regMsg.UserID = newUser.UserId;
                _user.UserID = newUser.UserId;
                SetUserData(_user, _msg =>
                {
                    if (_msg.IsSuccess)
                    {
                        _regMsg.IsComplete = true;
                        EasyProfileManager.Instance.RaiseRegistrationActions(_regMsg);
                        _callback.Invoke(_regMsg);
                        CleanTask(task);
                    }
                    else
                    {
                        _callback.Invoke(_regMsg);
                        CleanTask(task);
                    }
                });
                
            });
        }

        /// <summary>
        /// User registration. Use custom auth system
        /// </summary>
        /// <param name="_userID">Custom User ID</param>
        /// <param name="_user">User data</param>
        /// <param name="_callback">Add completion method</param>
        public void RegisterNewUserWithCustomAuthSystem(string _userID, User _user, Action<CallbackRegistrationMessage> _callback)
        {
            CallbackRegistrationMessage _regMsg = new CallbackRegistrationMessage();
            Auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    _regMsg.ErrorMessage = "SignInAnonymouslyAsync was canceled";
                    _callback.Invoke(_regMsg);
                    CleanTask(task);
                    return;
                }
                if (task.IsFaulted)
                {
                    _regMsg.ErrorMessage = "SignInAnonymouslyAsync encountered an error: " + task.Exception;
                    _callback.Invoke(_regMsg);
                    CleanTask(task);
                    return;
                }
                FirebaseUser newUser = task.Result;
                _regMsg.UserID = _userID;
                _user.UserID = _userID;
                SetUserData(_user, _msg =>
                {
                    if (_msg.IsSuccess)
                    {
                        _regMsg.IsComplete = true;
                        EasyProfileManager.Instance.RaiseRegistrationActions(_regMsg);
                        _callback.Invoke(_regMsg);
                        CleanTask(task);
                    }
                    else
                    {
                        _callback.Invoke(_regMsg);
                        CleanTask(task);
                    }
                });

            });
        }


        /// <summary>
        /// Sends an account activation letter to the user in the mail. Use this method after successful user registration.
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void SendVerifcationEmail(Action<CallbackSendVerificationEmail> _callback = null)
        {
            CallbackSendVerificationEmail _response = new CallbackSendVerificationEmail();
            FirebaseAuth.DefaultInstance.CurrentUser.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendEmailVerificationAsync was canceled.");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                }
                else
                {
                    _response.IsSuccess = true;
                    Debug.Log("Email sent successfully.");
                }
                if (_callback != null)
                {
                    _callback.Invoke(_response);
                }
            });
        }

        /// <summary>
        /// Sends a password reset email to the user.
        /// </summary>
        /// <param name="_mail">User email</param>
        /// <param name="_callback">Add completion method</param>
        public void SendResetPasswordEmail(string _mail, Action<CallbackSendResetPasswordEmail> _callback = null)
        {
            CallbackSendResetPasswordEmail _response = new CallbackSendResetPasswordEmail();
            FirebaseAuth.DefaultInstance.SendPasswordResetEmailAsync(_mail).ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                }
                else
                {
                    _response.IsSuccess = true;
                    Debug.Log("Password reset email sent successfully.");
                }
                if (_callback != null)
                {
                    _callback.Invoke(_response);
                }
            });
            
        }

        /// <summary>
        /// Create or set user data
        /// </summary>
        /// <param name="_user">User object</param>
        /// <param name="_callback">Add completion method</param>
        public void SetUserData(User _user, Action<CallbackSetUserDataMessage> _callback = null)
        {
            string json = JsonUtility.ToJson(_user);
            CallbackSetUserDataMessage _logMsg = new CallbackSetUserDataMessage();
            Database.RootReference.Child(Constants.FirebaseRootUsersKey).Child(_user.UserID).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    _logMsg.ErrorMessage = "Set user data was canceled";
                    _callback.Invoke(_logMsg);
                    CleanTask(task);
                    return;
                }
                if (task.IsFaulted)
                {
                    _logMsg.ErrorMessage = "Set User Data encountered an error: " + task.Exception;
                    _callback.Invoke(_logMsg);
                    CleanTask(task);
                    return;
                }
                _logMsg.IsSuccess = true;
                _logMsg.UserID = _user.UserID;
                EasyProfileManager.Instance.RaiseSetUserDataActions(_logMsg);
                if (_callback != null)
                {
                    _callback.Invoke(_logMsg);
                }
                CleanTask(task);
            });
        }

        /// <summary>
        /// Get user data
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserData(string _userID, Action<CallbackGetUserDataMessage> _callback)
        {
            CallbackGetUserDataMessage _response = new CallbackGetUserDataMessage();
            Query databaseQuery = Database.RootReference.Child(Constants.FirebaseRootUsersKey).Child(_userID);
            databaseQuery.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    User _user = JsonUtility.FromJson<User>(task.Result.GetRawJsonValue().ToString());
                    _response.IsSuccess = true;
                    _response.UserData = _user;
                    if (_callback != null)
                    {
                        _callback.Invoke(_response);
                    }
                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
                EasyProfileManager.Instance.RaiseGetUserDataActions(_response);
            });
        }

        /// <summary>
        /// Upload user avatar image to Firebase Storage. 
        /// </summary>
        /// <param name="_request">Request object. Use RequestUploadImage _request = new RequestUploadImage()</param>
        /// <param name="_callback">Add upload completion method</param>
        public void UploadAvatar(RequestUploadImage _request, Action<CallBackUploadImage> _callback = null)
        {
            CallBackUploadImage uploadCallback = new CallBackUploadImage();
            List<Task> TaskList = new List<Task>();
            ImageSize[] sizes = new ImageSize[] { ImageSize.Origin, ImageSize.Size_1024, ImageSize.Size_512, ImageSize.Size_256, ImageSize.Size_128 };
            foreach (ImageSize _s in sizes)
            {
                ImageSize _size = _s;
                if (_size != ImageSize.Origin)
                {
                    EasyProfileSolution.ResizeTexture(_request.Texture, _size);
                }
                byte[] uploadBytes = ImageConversion.EncodeToJPG(_request.Texture, EasyProfileManager.Instance.PROFILE_SETTING.UploadImageQuality);
                StorageReference avatar_ref = Storage.RootReference.Child(Constants.RootUserStorageKey).Child(_request.OwnerId).Child(Constants.UserStorageAvatarKey + "/" + "Image_" + _size.ToString() + ".jpg");
                Task _task = avatar_ref.PutBytesAsync(uploadBytes);
                TaskList.Add(_task);
            }

            Task.WhenAll(TaskList)
                .ContinueWithOnMainThread((task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        uploadCallback.IsFinish = true;
                        uploadCallback.IsSuccess = false;
                        _callback.Invoke(uploadCallback);
                        CleanTask(task);
                    }
                    else
                    {
                        uploadCallback.IsFinish = true;
                        uploadCallback.IsSuccess = true;
                        if (_callback != null)
                        {
                            _callback.Invoke(uploadCallback);
                        }
                        CleanTask(task);
                    }
                    EasyProfileManager.Instance.RaiseUploadAvatarActions(uploadCallback);
                });
        }

        /// <summary>
        /// Get dowload url of user avatar
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_size">Avatar size</param>
        /// <param name="_callback">Add completion method</param>
        public void GetProfileImageURL(string _userID, ImageSize _size , Action<CallbackGetProfileImageURL> _callback)
        {
            CallbackGetProfileImageURL _response = new CallbackGetProfileImageURL();
            StorageReference avatar_ref = Storage.RootReference.Child(Constants.RootUserStorageKey).Child(_userID).Child(Constants.UserStorageAvatarKey 
                + "/" + "Image_" + _size.ToString() + ".jpg");
            avatar_ref.GetDownloadUrlAsync().ContinueWithOnMainThread((Task<Uri> uriTask) =>
            {
                if (uriTask.IsFaulted || uriTask.IsCanceled)
                {
                    _response.IsSuccess = false;
                    _callback.Invoke(_response);
                    CleanTask(uriTask);
                }
                else
                {
                    string download_url = uriTask.Result.ToString();
                    _response.IsSuccess = true;
                    _response.DownloadUrl = download_url;
                    if (_callback != null)
                    {
                        _callback.Invoke(_response);
                    }
                    CleanTask(uriTask);
                }

            });
        }

        /// <summary>
        /// Get user avatar image from Firebase Storage.
        /// </summary>
        /// <param name="_request">Request object. Use RequestGetProfileImage _request = new RequestGetProfileImage()</param>
        /// <param name="_callback">Add completion method</param>
        public void GetProfileImage(RequestGetProfileImage _request, Action<CallbackGetProfileImage> _callback = null)
        {
            CallbackGetProfileImage _profileCallback = new CallbackGetProfileImage();
            const long maxAllowedSize = 1 * 2048 * 2048;
            StorageReference avatar_ref = Storage.RootReference.Child(Constants.RootUserStorageKey).Child(_request.Id).Child(Constants.UserStorageAvatarKey + "/" + "Image_" + _request.Size.ToString() + ".jpg");
            avatar_ref.GetBytesAsync(maxAllowedSize).ContinueWithOnMainThread((Task<byte[]> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    _profileCallback.IsSuccess = false;
                     _callback.Invoke(_profileCallback);
                    CleanTask(task);
                    EasyProfileManager.Instance.RaiseGetProfileImageActions(_profileCallback);
                }
                else
                {
                    avatar_ref.GetDownloadUrlAsync().ContinueWithOnMainThread((Task<Uri> uriTask) =>
                     {
                         if (uriTask.IsFaulted || uriTask.IsCanceled)
                         {
                             _profileCallback.IsSuccess = false;
                             _callback.Invoke(_profileCallback);
                             CleanTask(task);
                             EasyProfileManager.Instance.RaiseGetProfileImageActions(_profileCallback);
                         }
                         else
                         {
                             string download_url = uriTask.Result.ToString();
                             byte[] fileContents = task.Result;
                             _profileCallback.IsSuccess = true;
                             _profileCallback.ImageBytes = fileContents;
                             _profileCallback.DownloadUrl = download_url;
                             if (_callback != null)
                             {
                                 _callback.Invoke(_profileCallback);
                             }
                             if (EasyProfileManager.Instance.IsUserLogined())
                             {
                                 if (EasyProfileManager.Instance.PROFILE_CONTROLLER.IsMine(_request.Id))
                                 {
                                     Texture2D texture = new Texture2D(2, 2);
                                     texture.LoadImage(fileContents);
                                     EasyProfileManager.Instance.PROFILE_CONTROLLER.CACHED_AVATAR = texture;
                                 }
                             }
                             CleanTask(task);
                             
                             EasyProfileManager.Instance.RaiseGetProfileImageActions(_profileCallback);
                         }
                         
                     });
                    
                }
               
            });
        }

        /// <summary>
        /// Get the full name of user by id.
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserFullName(string _userID, Action<string> _callback)
        {
            Query databaseQuery = Database.RootReference.Child(Constants.FirebaseRootUsersKey).Child(_userID).Child(Constants.UserFullNameKey);
            databaseQuery.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _callback.Invoke(task.Result.Value.ToString());
                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }

        /// <summary>
        /// Set user full name by user ID.
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_value">Full name value</param>
        /// <param name="_callback">Add completion method</param>
        public void SetUserFullName(string _userID, string _value, Action<CallbackSetUserValue> _callback = null)
        {
            CallbackSetUserValue _response = new CallbackSetUserValue();
            DatabaseReference databaseReferense = Database.RootReference.Child(Constants.FirebaseRootUsersKey).Child(_userID).Child(Constants.UserFullNameKey);
            databaseReferense.SetValueAsync(_value).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _response.IsSuccess = true;
                    
                    CleanTask(task);
                }
                else
                {
                    _response.ErrorMessage = task.Exception.Message;
                    CleanTask(task);
                }
                if (_callback != null)
                {
                    _callback.Invoke(_response);
                }
            });
        }

        /// <summary>
        /// Get users last name by user ID.
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void GetUserLastName(string _userID, Action<string> _callback)
        {
            Query databaseQuery = Database.RootReference.Child(Constants.FirebaseRootUsersKey).Child(_userID).Child(Constants.UserLastNameKey);
            databaseQuery.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _callback.Invoke(task.Result.Value.ToString());
                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }

        /// <summary>
        /// Set users last name by user ID
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_value">Last name value</param>
        /// <param name="_callback">Add completion method</param>
        public void SetUserLastName(string _userID, string _value, Action<CallbackSetUserValue> _callback = null)
        {
            CallbackSetUserValue _response = new CallbackSetUserValue();
            DatabaseReference databaseReferense = Database.RootReference.Child(Constants.FirebaseRootUsersKey).Child(_userID).Child(Constants.UserLastNameKey);
            databaseReferense.SetValueAsync(_value).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _response.IsSuccess = true;

                    CleanTask(task);
                }
                else
                {
                    _response.ErrorMessage = task.Exception.Message;
                    CleanTask(task);
                }
                if (_callback != null)
                {
                    _callback.Invoke(_response);
                }
            });
        }

        /// <summary>
        /// Get the first name of user by id.
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserFirstName(string _userID, Action<string> _callback)
        {
            Query databaseQuery = Database.RootReference.Child(Constants.FirebaseRootUsersKey).Child(_userID).Child(Constants.UserFirstNameKey);
            databaseQuery.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _callback.Invoke(task.Result.Value.ToString());
                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }

        /// <summary>
        /// Set users first name by user ID
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_value">First name value</param>
        /// <param name="_callback"></param>
        public void SetUserFirstName(string _userID, string _value, Action<CallbackSetUserValue> _callback = null)
        {
            CallbackSetUserValue _response = new CallbackSetUserValue();
            DatabaseReference databaseReferense = Database.RootReference.Child(Constants.FirebaseRootUsersKey).Child(_userID).Child(Constants.UserFirstNameKey);
            databaseReferense.SetValueAsync(_value).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _response.IsSuccess = true;

                    CleanTask(task);
                }
                else
                {
                    _response.ErrorMessage = task.Exception.Message;
                    CleanTask(task);
                }
                if (_callback != null)
                {
                    _callback.Invoke(_response);
                }
            });
        }

        /// <summary>
        /// Get the nick name of user by id.
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserNickName(string _userID, Action<string> _callback)
        {
            Query databaseQuery = Database.RootReference.Child(Constants.FirebaseRootUsersKey).Child(_userID).Child(Constants.UserNickNameKey);
            databaseQuery.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _callback.Invoke(task.Result.Value.ToString());
                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }

        /// <summary>
        /// Set user nick name by user ID
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_value">Nick name value</param>
        /// <param name="_callback">Add completion method</param>
        public void SetUserNickName(string _userID, string _value, Action<CallbackSetUserValue> _callback = null)
        {
            CallbackSetUserValue _response = new CallbackSetUserValue();
            DatabaseReference databaseReferense = Database.RootReference.Child(Constants.FirebaseRootUsersKey).Child(_userID).Child(Constants.UserNickNameKey);
            databaseReferense.SetValueAsync(_value).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _response.IsSuccess = true;

                    CleanTask(task);
                }
                else
                {
                    _response.ErrorMessage = task.Exception.Message;
                    CleanTask(task);
                }
                if (_callback != null)
                {
                    _callback.Invoke(_response);
                }
            });
        }

        /// <summary>
        /// Get user registration date by user ID
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserRegistrationDate(string _userID, Action<string> _callback)
        {
            Query databaseQuery = Database.RootReference.Child(Constants.FirebaseRootUsersKey).Child(_userID).Child(Constants.UserRegistrationDateKey);
            databaseQuery.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _callback.Invoke(task.Result.Value.ToString());
                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }

        /// <summary>
        /// Get user phone number date by user ID
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserPhone(string _userID, Action<string> _callback)
        {
            Query databaseQuery = Database.RootReference.Child(Constants.FirebaseRootUsersKey).Child(_userID).Child(Constants.UserPhoneKey);
            databaseQuery.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _callback.Invoke(task.Result.Value.ToString());
                    CleanTask(task);
                }
                else
                {
                    CleanTask(task);
                }
            });
        }

        /// <summary>
        /// Set user phone
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_value">Phone value</param>
        /// <param name="_callback">Add completion method</param>
        public void SetUserPhone(string _userID, string _value, Action<CallbackSetUserValue> _callback = null)
        {
            CallbackSetUserValue _response = new CallbackSetUserValue();
            DatabaseReference databaseReferense = Database.RootReference.Child(Constants.FirebaseRootUsersKey).Child(_userID).Child(Constants.UserPhoneKey);
            databaseReferense.SetValueAsync(_value).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _response.IsSuccess = true;

                    CleanTask(task);
                }
                else
                {
                    _response.ErrorMessage = task.Exception.Message;
                    CleanTask(task);
                }
                if (_callback != null)
                {
                    _callback.Invoke(_response);
                }
            });
        }

        /// <summary>
        /// Get all user of Firebase Database
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void GetAllUsers (Action<CallbackGetAllUsers> _callback)
        {
            CallbackGetAllUsers _response = new CallbackGetAllUsers();
            Query databaseQuery = Database.RootReference.Child(Constants.FirebaseRootUsersKey);
            databaseQuery.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    _response.IsSuccess = false;
                    CleanTask(task);
                }
                else if (task.IsCompleted && task.Result.Exists)
                {
                    _response.IsSuccess = true;
                    DataSnapshot snapshot = task.Result;
                    List<User> _users = new List<User>();

                    int _userCount = (int)snapshot.ChildrenCount;

                    for (int i = 0; i < _userCount; i++)
                    {
                        DataSnapshot _userSnapshot = snapshot.Children.ElementAt(i);
                        User _userData = JsonUtility.FromJson<User>(_userSnapshot.GetRawJsonValue());
                        _users.Add(_userData);
                    }
                    _response.Users = _users;
                    CleanTask(task);
                }
                _callback.Invoke(_response);
            });
        }

        /// <summary>
        /// Search users by string value. You can search for users by name, nickname or phone number.
        /// </summary>
        /// <param name="_searchValue">Search Value</param>
        /// <param name="_callback">Add completion method</param>
        public void SearchUsers(string _search, Action<CallbackSearchUsers> _callback)
        {
            CallbackSearchUsers _response = new CallbackSearchUsers();

            Query databaseQueryFirstName = Database.RootReference.Child(Constants.FirebaseRootUsersKey).OrderByChild(Constants.UserFirstNameKey).StartAt(_search).EndAt(_search + "\uf8ff");
            Query databaseQueryLastName = Database.RootReference.Child(Constants.FirebaseRootUsersKey).OrderByChild(Constants.UserLastNameKey).StartAt(_search).EndAt(_search + "\uf8ff");
            Query databaseQueryFullName = Database.RootReference.Child(Constants.FirebaseRootUsersKey).OrderByChild(Constants.UserFullNameKey).StartAt(_search).EndAt(_search + "\uf8ff");
            Query databaseQueryNickName = Database.RootReference.Child(Constants.FirebaseRootUsersKey).OrderByChild(Constants.UserNickNameKey).StartAt(_search).EndAt(_search + "\uf8ff");
            Query databaseQueryPhone = Database.RootReference.Child(Constants.FirebaseRootUsersKey).OrderByChild(Constants.UserPhoneKey).StartAt(_search).EndAt(_search + "\uf8ff");


            List<Task> TaskList = new List<Task>();

            TaskList.Add(databaseQueryFirstName.GetValueAsync());
            TaskList.Add(databaseQueryLastName.GetValueAsync());
            TaskList.Add(databaseQueryFullName.GetValueAsync());
            TaskList.Add(databaseQueryNickName.GetValueAsync());
            TaskList.Add(databaseQueryPhone.GetValueAsync());

            Task.WhenAll(TaskList).ContinueWithOnMainThread(task2 =>
            {
                if (task2.IsCompleted)
                {
                    _response.IsSuccess = true;
                    List<User> users = new List<User>();
                    List<string> usersKeys = new List<string>();
                    foreach (Task<DataSnapshot> t in TaskList)
                    {
                        if (t.IsCompleted && t.Result.Exists)
                        {
                            DataSnapshot snapshot = t.Result;

                            for (int i = 0; i < snapshot.ChildrenCount; i++)
                            {
                                DataSnapshot userSnapshot = snapshot.Children.ElementAt(i);
                                string jsonMessage = userSnapshot.GetRawJsonValue();
                                if (!string.IsNullOrEmpty(jsonMessage))
                                {
                                    User _dataUser = JsonUtility.FromJson<User>(jsonMessage);
                                    if (_dataUser != null)
                                    {
                                        if (!usersKeys.Contains(userSnapshot.Key))
                                        {
                                            users.Add(_dataUser);
                                            usersKeys.Add(userSnapshot.Key);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    _response.Users = users;
                    _callback.Invoke(_response);
                    CleanTask(task2);
                }

            });
        }

        /// <summary>
        /// Set user custom value by user ID. All custom values must be configured in plugin settings (EasySystems->EasyProfile->Settings)
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_valueID">Custom value id</param>
        /// <param name="_value">Custom value</param>
        /// <param name="_callback">Add completion method</param>
        public void SetUserCustomValue(string _userID ,string _valueID, object _value, Action<CallbackSetUserCustomValue> _callback = null)
        {
            CallbackSetUserCustomValue _response = new CallbackSetUserCustomValue();
            DatabaseReference databaseReferense = Database.RootReference.Child(Constants.FirebaseRootUsersKey)
                .Child(_userID).Child(Constants.UserCustomValueKey).Child(_valueID);
            databaseReferense.SetValueAsync(_value).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _response.ValueID = _valueID;
                    _response.IsSuccess = true;
                    _response.UserID = _userID;
                    CleanTask(task);
                }
                else
                {
                    _response.ErrorMessage = task.Exception.Message;
                    CleanTask(task);
                }
                if (_callback != null)
                {
                    _callback.Invoke(_response);
                }
                EasyProfileManager.Instance.RaiseOnSetCustomDataActions(_response);
            });
        }

        /// <summary>
        /// Get user custom value by user ID. All custom values must be configured in plugin settings (EasySystems->EasyProfile->Settings)
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_valueID">Value ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserCustomValue(string _userID, string _valueID, Action<CallbackGetUserCustomValue> _callback)
        {
            CallbackGetUserCustomValue _response = new CallbackGetUserCustomValue();
            DatabaseReference databaseReferense = Database.RootReference.Child(Constants.FirebaseRootUsersKey)
                .Child(_userID).Child(Constants.UserCustomValueKey).Child(_valueID);
            databaseReferense.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _response.UserID = _userID;
                    _response.ValueID = _valueID;
                    if (task.Result.Exists)
                    {
                        _response.IsSuccess = true;
                        _response.CustomValue = new CustomValue();
                        CustomValueType _currentType = EasyProfileManager.Instance.PROFILE_SETTING.GetValueTypeByKey(_valueID);
                        switch (_currentType)
                        {
                            case CustomValueType.String:
                                _response.CustomValue.StringValue = task.Result.Value.ToString();
                                break;
                            case CustomValueType.Int:
                                _response.CustomValue.IntValue = int.Parse(task.Result.Value.ToString());
                                break;
                            case CustomValueType.Float:
                                _response.CustomValue.FloatValue = float.Parse(task.Result.Value.ToString());
                                break;
                        }
                        _response.CustomValue.ValueID = _valueID;
                    }                    
                    CleanTask(task);
                }
                else
                {
                    _response.ErrorMessage = task.Exception.Message;
                    CleanTask(task);
                }
                if (_callback != null)
                {
                    _callback.Invoke(_response);
                }
                EasyProfileManager.Instance.RaiseOnGetCustomDataActions(_response);
            });
        }

        /// <summary>
        /// Update user last activity value
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void UpdateUserActivity(Action<CallbackSetUserActivity> _callback = null)
        {
            CallbackSetUserActivity _response = new CallbackSetUserActivity();
            DatabaseReference databaseReferense = Database.RootReference.Child(Constants.FirebaseRootUsersKey)
                .Child(EasyProfileManager.Instance.PROFILE_CONTROLLER.USER_ID).Child(Constants.UserActivityKey);
            databaseReferense.SetValueAsync(ServerValue.Timestamp).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _response.IsSuccess = true;
                    CleanTask(task);
                }
                else
                {
                    _response.ErrorMessage = task.Exception.Message;
                    CleanTask(task);
                }
                if (_callback != null)
                {
                    _callback.Invoke(_response);
                }
            });
        }

        /// <summary>
        /// Get current server time. Availbale when user is logined
        /// </summary>
        /// <param name="_callback">Add completion method</param>
        public void GetServerTimestamp(Action<CallbackGetServerTimestamp> _callback)
        {
            CallbackGetServerTimestamp _response = new CallbackGetServerTimestamp();
            UpdateUserActivity(_msg =>
            {
                if (_msg.IsSuccess)
                {
                    DatabaseReference databaseReferense = Database.RootReference.Child(Constants.FirebaseRootUsersKey)
                    .Child(EasyProfileManager.Instance.PROFILE_CONTROLLER.USER_ID).Child(Constants.UserActivityKey);
                    databaseReferense.GetValueAsync().ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCompleted)
                        {
                            _response.IsSuccess = true;
                            _response.Data = task.Result.Value.ToString();
                            CleanTask(task);
                        }
                        else
                        {
                            _response.ErrorMessage = task.Exception.Message;
                            CleanTask(task);
                        }
                        if (_callback != null)
                        {
                            _callback.Invoke(_response);
                        }
                    });
                }
                else
                {
                    if (_callback != null)
                    {
                        _callback.Invoke(_response);
                    }
                }
            });
            
            
        }

        /// <summary>
        /// Get user last activity by user id.
        /// </summary>
        /// <param name="_userID">User ID</param>
        /// <param name="_callback">Add completion method</param>
        public void GetUserLastActivity(string _userID, Action<CallbackGetUserActivity> _callback)
        {
            CallbackGetUserActivity _response = new CallbackGetUserActivity();
            DatabaseReference databaseReferense = Database.RootReference.Child(Constants.FirebaseRootUsersKey)
                .Child(_userID).Child(Constants.UserActivityKey);
            databaseReferense.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    _response.IsSuccess = true;
                    _response.Data = task.Result.Value.ToString();
                    CleanTask(task);
                }
                else
                {
                    _response.ErrorMessage = task.Exception.Message;
                    CleanTask(task);
                }
                if (_callback != null)
                {
                    _callback.Invoke(_response);
                }
            });
        }
    }

    /// <summary>
    /// Login callback class
    /// </summary>
    public class CallbackLoginMessage
    {
        public bool IsSuccess;
        public string ErrorMessage;
        public ProfileMessageCode MessageCode;
        public string UserID;
        public UserCredentials Gredentials;
        public FirebaseUser FUser;
    }

    /// <summary>
    /// Registration callback class
    /// </summary>
    public class CallbackRegistrationMessage
    {
        public bool IsComplete;
        public string ErrorMessage;
        public string UserID;
    }

    /// <summary>
    /// Set user data callback class
    /// </summary>
    public class CallbackSetUserDataMessage
    {
        public bool IsSuccess;
        public string ErrorMessage;
        public string UserID;
    }

    /// <summary>
    /// Set user custom valuse callback class
    /// </summary>
    public class CallbackSetUserValue
    {
        public bool IsSuccess;
        public string ErrorMessage;
    }

    /// <summary>
    /// Get user data callback class
    /// </summary>
    public class CallbackGetUserDataMessage
    {
        public bool IsSuccess;
        public User UserData;
    }

    /// <summary>
    /// Init Firebase callback class
    /// </summary>
    public class CallbackInitFirebaseMessage
    {
        public bool IsSuccess;
    }

    /// <summary>
    /// Upload image request class
    /// </summary>
    public class RequestUploadImage
    {
        public Texture2D Texture;
        public string OwnerId;
    }

    /// <summary>
    /// Upload image callback class
    /// </summary>
    public class CallBackUploadImage
    {
        public bool IsFinish = false;
        public bool IsSuccess = false;
    }

    /// <summary>
    /// Get profile image request class
    /// </summary>
    public class RequestGetProfileImage
    {
        public string Id;
        public ImageSize Size;
    }

    /// <summary>
    /// Get profile image callback class
    /// </summary>
    public class CallbackGetProfileImage
    {
        public bool IsSuccess;
        public byte[] ImageBytes;
        public string DownloadUrl;
    }

    /// <summary>
    /// Get profile image callback class
    /// </summary>
    public class CallbackGetProfileImageURL
    {
        public bool IsSuccess;
        public string DownloadUrl;
    }

    /// <summary>
    /// Log out callback class
    /// </summary>
    public class CallbackLogOut
    {
        public bool IsSuccess;
    }

    /// <summary>
    /// Get all user callback class
    /// </summary>
    public class CallbackGetAllUsers
    {
        public bool IsSuccess;
        public List<User> Users;
    }

    /// <summary>
    /// Search users callback class
    /// </summary>
    public class CallbackSearchUsers
    {
        public bool IsSuccess;
        public List<User> Users;
    }

    /// <summary>
    /// Send verification email class
    /// </summary>
    public class CallbackSendVerificationEmail
    {
        public bool IsSuccess;
    }

    /// <summary>
    /// Send reset password email
    /// </summary>
    public class CallbackSendResetPasswordEmail
    {
        public bool IsSuccess;
    }

    /// <summary>
    /// Set user custom value callback class
    /// </summary>
    public class CallbackSetUserCustomValue
    {
        public bool IsSuccess;
        public string ErrorMessage;
        public string UserID;
        public string ValueID;
    }

    /// <summary>
    /// Get user custom value callback class
    /// </summary>
    public class CallbackGetUserCustomValue
    {
        public bool IsSuccess;
        public string ErrorMessage;
        public string UserID;
        public string ValueID;
        public CustomValue CustomValue;
    }

    /// <summary>
    /// Set user activity callback class
    /// </summary>
    public class CallbackSetUserActivity
    {
        public bool IsSuccess;
        public string ErrorMessage;
    }

    /// <summary>
    /// Get server time stamp callback class
    /// </summary>
    public class CallbackGetServerTimestamp
    {
        public bool IsSuccess;
        public string ErrorMessage;
        public string Data;
    }

    /// <summary>
    /// Get user activity callback class
    /// </summary>
    public class CallbackGetUserActivity
    {
        public bool IsSuccess;
        public string ErrorMessage;
        public string Data;
    }

    /// <summary>
    /// Custom value serializable class
    /// </summary>
    [System.Serializable]
    public class CustomValue
    {
        public string ValueID;
        public CustomValueType ValueType;

        [HideInInspector]
        public int IntValue;
        [HideInInspector]
        public string StringValue;
        [HideInInspector]
        public float FloatValue;
    }

    /// <summary>
    /// Enum for custom user value type
    /// </summary>
    public enum CustomValueType
    {
        String,
        Int,
        Float
    }

    /// <summary>
    /// Enum for texture resizing 
    /// </summary>
    public enum ImageSize
    {
        Origin = 0,
        Size_1024 = 1024,
        Size_512 = 512,
        Size_256 = 256,
        Size_128 = 128
    }

}

