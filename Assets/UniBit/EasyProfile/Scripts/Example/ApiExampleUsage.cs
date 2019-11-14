using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;

namespace EasyProfile
{
    public class ApiExampleUsage : MonoBehaviour
    {

        private void Start()
        {
            AddListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            // add all listeners
            EasyProfileManager.Instance.AddOnInitAction(OnFirebaseInited);
            EasyProfileManager.Instance.AddOnLoginAction(OnUserLogined);
            EasyProfileManager.Instance.AddOnRegistrationAction(OnRegistrationComplete);
            EasyProfileManager.Instance.AddOnSetUserDataCompleteAction(OnSetUserDataComplete);
            EasyProfileManager.Instance.AddOnGetUserDataCompleteAction(OnGetUserDataComplete);
            EasyProfileManager.Instance.AddOnUploadAvatarCompleteAction(OnUploadAvatarImageComplete);
            EasyProfileManager.Instance.AddOnGetProfileImageAction(OnGetProfileImageComplete);
            EasyProfileManager.Instance.AddOnLogOutAction(OnLogOut);
            EasyProfileManager.Instance.AddOnSetCustomDataAction(OnSetCustomValueComplete);
            EasyProfileManager.Instance.AddOnGetCustomDataAction(OnGetCustomValueComplete);
        }

        private void RemoveListeners()
        {
            // remove all listeners
            EasyProfileManager.Instance.RemoveOnInitAction(OnFirebaseInited);
            EasyProfileManager.Instance.RemoveOnLoginAction(OnUserLogined);
            EasyProfileManager.Instance.RemoveOnRegistrationAction(OnRegistrationComplete);
            EasyProfileManager.Instance.RemoveOnSetUserDataCompleteAction(OnSetUserDataComplete);
            EasyProfileManager.Instance.RemoveOnGetUserDataCompleteAction(OnGetUserDataComplete);
            EasyProfileManager.Instance.RemoveOnUploadCompleteAction(OnUploadAvatarImageComplete);
            EasyProfileManager.Instance.RemoveOnGetProfileImageAction(OnGetProfileImageComplete);
            EasyProfileManager.Instance.RemoveOnLogOutAction(OnLogOut);
            EasyProfileManager.Instance.RemoveOnSetCustomDataAction(OnSetCustomValueComplete);
            EasyProfileManager.Instance.RemoveOnGetCustomDataAction(OnGetCustomValueComplete);
        }

        private void TestsCalls()
        {
            // get system data
            string currentDate = EasyProfileManager.Instance.GetSystemDate();
            // save user login data
            EasyProfileManager.Instance.SaveUserCredentials("test@gmail.com", "password123");
            // check if has user login data
            bool hasSavedCredendtials = EasyProfileManager.Instance.HasSavedCredentials();
            // clear user login data
            EasyProfileManager.Instance.ClearUserCredentials();
            // get saved user login data
            UserCredentials _userCred = EasyProfileManager.Instance.GetSavedCredentials();
            Debug.Log("Login" + _userCred.UserLogin);
            Debug.Log("Password" + _userCred.UserPassword);
            // init firebase
            EasyProfileManager.Instance.InitFirebase(OnFirebaseInited);
            // check is firebase is inited
            bool IsFirebaseInited = EasyProfileManager.Instance.IsFirebaseInited();
            // register new user
            EasyProfileManager.Instance.RegisterNewUser("test@gmail.com", "password123", OnRegistrationComplete);
            // register new user with custom token id
            User _newUser = new User();
            _newUser.DataRegistration = EasyProfileManager.Instance.GetSystemDate();
            _newUser.FirstName = "First Name";
            _newUser.FullName = "Full Name";
            _newUser.LastName = "Last Name";
            _newUser.NickName = "NickName";
            _newUser.Phone = "+38093333333";
            EasyProfileManager.Instance.RegisterNewUserWithCustomTokenID("your custom token id", _newUser, OnRegistrationComplete);
            // register new user with custom auth system
            User _newUser3 = new User();
            _newUser3.DataRegistration = EasyProfileManager.Instance.GetSystemDate();
            _newUser3.FirstName = "First Name";
            _newUser3.FullName = "Full Name";
            _newUser3.LastName = "Last Name";
            _newUser3.NickName = "NickName";
            _newUser3.Phone = "+38093333333";
            EasyProfileManager.Instance.RegisterNewUserWithCustomAuthSystem("your user id", _newUser3, OnRegistrationComplete);
            // register new user with Credential object
            UserCredentials _userCredential = new UserCredentials();
            _userCredential.UserLogin = "test@gmail.com";
            _userCredential.UserPassword = "password123";
            EasyProfileManager.Instance.RegisterNewUser(_userCredential, OnRegistrationComplete);
            // login user
            EasyProfileManager.Instance.LogIn("test@gmail.com", "password123", OnUserLogined);
            // login user with Credential object
            UserCredentials _userCredential2 = new UserCredentials();
            _userCredential2.UserLogin = "test@gmail.com";
            _userCredential2.UserPassword = "password123";
            EasyProfileManager.Instance.LogIn(_userCredential2, OnUserLogined);
            // login user with custom token id
            EasyProfileManager.Instance.LogInWithCustomTokenID("your custom token id", OnUserLogined);
            // login user with custom auth system
            EasyProfileManager.Instance.LogInWithCustomAuthSystem("your user id", OnUserLogined);
            // upload image avatar
            Texture2D TestTexture = null;
            RequestUploadImage _imageUploadRequset = new RequestUploadImage();
            _imageUploadRequset.OwnerId = EasyProfileManager.Instance.PROFILE_CONTROLLER.USER_ID;
            _imageUploadRequset.Texture = TestTexture;
            EasyProfileManager.Instance.UploadAvatar(_imageUploadRequset, OnUploadAvatarImageComplete);
            // get image avatar
            RequestGetProfileImage _imageGetRequest = new RequestGetProfileImage();
            _imageGetRequest.Id = EasyProfileManager.Instance.PROFILE_CONTROLLER.USER_ID;
            _imageGetRequest.Size = ImageSize.Size_512;
            EasyProfileManager.Instance.GetProfileImage(_imageGetRequest, OnGetProfileImageComplete);
            // get user data
            EasyProfileManager.Instance.GetUserData(OnGetUserDataComplete);
            // get user data by user id
            EasyProfileManager.Instance.GetUserDataByID(EasyProfileManager.Instance.PROFILE_CONTROLLER.USER_ID, OnGetUserDataComplete);
            // send verification mail
            EasyProfileManager.Instance.SendMailVerification(OnVerificationEmailSended);
            // send reset password
            EasyProfileManager.Instance.SendResetPasswordEmail("test@gmail.com", OnResetPasswordToEmailSended);
            // set user data
            User _newUser2 = new User();
            _newUser2.UserID = "userID"; // get it from registration complete message or login complete message
            _newUser2.DataRegistration = EasyProfileManager.Instance.GetSystemDate();
            _newUser2.FirstName = "First Name";
            _newUser2.FullName = "Full Name";
            _newUser2.LastName = "Last Name";
            _newUser2.NickName = "NickName";
            _newUser2.Phone = "+38093333333";
            EasyProfileManager.Instance.SetUserData(_newUser2, OnSetUserDataComplete);
            // get/set user full name
            EasyProfileManager.Instance.GetUserFullName(OnGetUserFullName);
            EasyProfileManager.Instance.GetUserFullNameByID("userID", OnGetUserFullName);
            EasyProfileManager.Instance.SetUserFullName("new value", OnSetUserValueComplete);
            // get/set user last name
            EasyProfileManager.Instance.GetUserLastName(OnGetUserLastName);
            EasyProfileManager.Instance.GetUserLastNameByID("userID", OnGetUserLastName);
            EasyProfileManager.Instance.SetUserLastName("new value", OnSetUserValueComplete);
            // get/set user first name
            EasyProfileManager.Instance.GetUserFirstName(OnGetUserFirstName);
            EasyProfileManager.Instance.GetUserFirstNameByID("userID", OnGetUserFirstName);
            EasyProfileManager.Instance.SetUserFirstName("new value", OnSetUserValueComplete);
            // get/set user nick name
            EasyProfileManager.Instance.GetUserNickName(OnGetUserNickName);
            EasyProfileManager.Instance.GetUserNickNameByID("userID", OnGetUserNickName);
            EasyProfileManager.Instance.SetUserNickName("new value", OnSetUserValueComplete);
            // get/set user phone number
            EasyProfileManager.Instance.GetUserPhone(OnGetUserPhone);
            EasyProfileManager.Instance.GetUserPhoneByID("userID", OnGetUserPhone);
            EasyProfileManager.Instance.SetUserPhone("new value", OnSetUserValueComplete);
            // get user registration date
            EasyProfileManager.Instance.GetUserRegistrationDate(OnGetRegistrationDate);
            EasyProfileManager.Instance.GetUserRegistrationDateByID("userID", OnGetRegistrationDate);
            // get all users
            EasyProfileManager.Instance.GetAllUsers(OnGetAllUsers);
            // log out
            EasyProfileManager.Instance.LogOut(OnLogOut);
            // search user
            EasyProfileManager.Instance.SearchUsers("Jack", OnSearchUserComplete);
            // set user custom data
            EasyProfileManager.Instance.SetCustomValue("Gold", 150, OnSetCustomValueComplete);
            // set user custom data by user id
            EasyProfileManager.Instance.SetCustomValueByUserID("UserID" ,"Gold", 150, OnSetCustomValueComplete);
            // get user custom data
            EasyProfileManager.Instance.GetCustomValue("Gold", OnGetCustomValueComplete);
            // get user custom data by user id
            EasyProfileManager.Instance.GetCustomValueByUserID("UserID", "Gold", OnGetCustomValueComplete);
            // update activity
            EasyProfileManager.Instance.UpdateUserActivity(OnUpdateActivityComplete);
            // get user activity
            EasyProfileManager.Instance.GetUserLastActivity("UserID", OnActivityGetted);
            // get server timestamp
            EasyProfileManager.Instance.GetServerTimestamp(OnServerTimeStampGetted);


            if (EasyProfileManager.Instance.PROFILE_CONTROLLER.IsLogined())
            {
                // get last saved user data
                User _user = EasyProfileManager.Instance.PROFILE_CONTROLLER.CURRENT_USER_DATA;
                Debug.Log(_user.NickName);
                // get last cached user avatar
                if (EasyProfileManager.Instance.PROFILE_CONTROLLER.PROFILE_IMAGE_LOADED)
                {
                    Texture2D _avatarTexture = EasyProfileManager.Instance.PROFILE_CONTROLLER.CACHED_AVATAR;
                }
                // get firebase user object
                FirebaseUser fUSer = EasyProfileManager.Instance.PROFILE_CONTROLLER.FIREBASE_USER;
                Debug.Log(fUSer.UserId);
                string UserID = EasyProfileManager.Instance.PROFILE_CONTROLLER.USER_ID;
            }
            // show login windows
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowLogin();
            // show loading windows
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowLoading();
            // show popup windows
            PopupMessage _popupMSG = new PopupMessage();
            _popupMSG.Callback = OnPopupMessageClosed;
            _popupMSG.Title = "Some Title";
            _popupMSG.Message = "Some Message";
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowPopupMessage(_popupMSG);
            // show registration windows
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowRegistration();
            // show restore password windows
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowRestorePassword();
            // show user profile windows
            EasyProfileManager.Instance.VIEW_CONTROLLER.ShowUserProfileView();
        }

        private void OnFirebaseInited(CallbackInitFirebaseMessage _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Init Firebase Success");
            }
            else
            {
                Debug.Log("Failed to init Firebase");
            }
        }

        private void OnUserLogined(CallbackLoginMessage _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Log in success. User id = "+_msg.UserID);
            }
            else
            {
                Debug.Log("Failed to log in "+_msg.ErrorMessage);
            }
        }

        private void OnRegistrationComplete(CallbackRegistrationMessage _msg)
        {
            if (_msg.IsComplete)
            {
                Debug.Log("Registration success! New user ID = "+ _msg.UserID);
            }
            else
            {
                Debug.Log("Failed to register user. "+_msg.ErrorMessage);
            }
        }

        private void OnSetUserDataComplete(CallbackSetUserDataMessage _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Set user data success");
            }
            else
            {
                Debug.Log("Set user data failed");
            }
        }

        private void OnGetUserDataComplete(CallbackGetUserDataMessage _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Get user data success");
                Debug.Log(_msg.UserData.FullName+ "," + _msg.UserData.UserID +"," + _msg.UserData.NickName);
            }
            else
            {
                Debug.Log("Set user data failed");
            }
        }

        private void OnUploadAvatarImageComplete(CallBackUploadImage _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Upload user image success");
            }
            else
            {
                Debug.Log("Upload user image failed");
            }
        }

        private Image AvatarSprite;

        private void OnGetProfileImageComplete(CallbackGetProfileImage _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Get Image profile success");
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(_msg.ImageBytes);
                AvatarSprite.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
            else
            {
                Debug.Log("Failed to load avatar");
            }
        }

        private void OnLogOut(CallbackLogOut _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Log out success!");
            }
            else
            {
                Debug.Log("Log out failed");
            }
        }

        private void OnSetCustomValueComplete(CallbackSetUserCustomValue _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Set user custom value success.Value ID = "+_msg.ValueID);
            }
            else
            {
                Debug.Log("Failed to set user custom value");
            }
        }

        private void OnGetCustomValueComplete(CallbackGetUserCustomValue _msg)
        {
            if (_msg.IsSuccess)
            {
                if (_msg.ValueID == "Gold")
                {
                    int _gold = _msg.CustomValue.IntValue;
                }
                if (_msg.ValueID == "Exp")
                {
                    float _exp = _msg.CustomValue.FloatValue;
                }
                if (_msg.ValueID == "Status")
                {
                    string _status = _msg.CustomValue.StringValue;
                }
            }
            else
            {
                Debug.Log("Failed to get user custom value");
            }
        }

        private void OnVerificationEmailSended(CallbackSendVerificationEmail _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Verification Email Sended Success");
            }
            else
            {
                Debug.Log("Failed to send email");
            }
        }

        private void OnResetPasswordToEmailSended(CallbackSendResetPasswordEmail _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Reset Password Email Sended Success");
            }
            else
            {
                Debug.Log("Failed to send email");
            }
        }

        private void OnSetUserValueComplete(CallbackSetUserValue _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Update user value success");
            }
            else
            {
                Debug.Log("Failed to update user value");
            }
        }

        private void OnGetUserFullName(string _msg)
        {
            Debug.Log("Full name = "+_msg);
        }

        private void OnGetUserLastName(string _msg)
        {
            Debug.Log("Last name = " + _msg);
        }

        private void OnGetUserFirstName(string _msg)
        {
            Debug.Log("First name = " + _msg);
        }

        private void OnGetUserNickName(string _msg)
        {
            Debug.Log("Nick name = " + _msg);
        }

        private void OnGetUserPhone(string _msg)
        {
            Debug.Log("Phone = " + _msg);
        }

        private void OnGetRegistrationDate(string _msg)
        {
            Debug.Log("Registration date = " + _msg);
        }

        private void OnGetAllUsers(CallbackGetAllUsers _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Success to load all users");
                foreach (User _user in _msg.Users)
                {
                    Debug.Log(_user.NickName);
                }
            }
            else
            {
                Debug.Log("Failed to load all users");
            }
        }

        private void OnSearchUserComplete(CallbackSearchUsers _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Success to search users");
                foreach (User _user in _msg.Users)
                {
                    Debug.Log(_user.NickName);
                }
            }
            else
            {
                Debug.Log("Failed to search users");
            }
        }

        private void OnPopupMessageClosed()
        {
            Debug.Log("Popup closed");
        }

        private void OnUpdateActivityComplete(CallbackSetUserActivity _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Update Activity Complete");
            }
        }

        private void OnActivityGetted(CallbackGetUserActivity _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Last user activity = " + _msg.Data);
            }
        }

        private void OnServerTimeStampGetted(CallbackGetServerTimestamp _msg)
        {
            if (_msg.IsSuccess)
            {
                Debug.Log("Server timestamp = " + _msg.Data);
            }
        }
    }
}

