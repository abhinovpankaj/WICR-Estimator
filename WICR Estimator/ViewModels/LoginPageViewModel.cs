using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security;
using System.Windows.Controls;
using WICR_Estimator.DBModels;
using WICR_Estimator.Services;

namespace WICR_Estimator.ViewModels
{
    internal class LoginPageViewModel :BaseViewModel, IPageViewModel
    {
        public bool SaveCredentials { get; set; }

       
        public LoginPageViewModel(IDialogCoordinator instance)
        {
            
            if (Properties.Settings.Default.SaveCredentials)
            {
                Username = Properties.Settings.Default.Username;
                Password = Properties.Settings.Default.Password;
            }
                
        }
        public LoginPageViewModel()
        {
            Username = "";
            Password = "";

            if (Properties.Settings.Default.SaveCredentials)
            {
                Username = Properties.Settings.Default.Username;
                Password = Properties.Settings.Default.Password;
                IsAdmin= Properties.Settings.Default.IsAdmin;
                SaveCredentials = Properties.Settings.Default.SaveCredentials;
                OnPropertyChanged("SaveCredentials");
                if (Username.Length>0 && Password.Length>0)
                {
                    UserDB user = new UserDB();
                    user.Username = Username;
                    user.IsAdmin = IsAdmin;
                    if (OnLoggedIn != null)
                    {
                        OnLoggedIn(user, EventArgs.Empty);

                    }
                }
            }

        }
        private string Password { get; set; }

        public static event EventHandler OnLoggedIn;
        public static event EventHandler ProgressStarted;
        public string Name => "Login Page";
        private bool _loginFailed;
        public bool LoginFailed
        {
            get { return _loginFailed; }
            set
            {
                _loginFailed = value;
                OnPropertyChanged("FailedLogin");
            }
        }
        //Property to hold the error message to be displayed
        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (value != _errorMessage)
                {
                    _errorMessage = value;
                    OnPropertyChanged("ErrorMessage");
                }
            }
        }
        private bool _isadmin;
        public bool IsAdmin
        {
            get { return _isadmin; }
            set
            {
                if (_isadmin!=value)
                {
                    _isadmin = value;
                    OnPropertyChanged("IsAdmin");
                }
            }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                if (!string.Equals(value.ToString(), _username, StringComparison.OrdinalIgnoreCase))
                {
                    _username = value;
                    OnPropertyChanged("Username");
                }
            }
        }
        private SecureString _password;
        public SecureString PasswordSecureString
        {
            get { return _password; }
            set
            {
                if (value != null)
                {
                    _password = value;
                    OnPropertyChanged("PasswordSecureString");
                }
            }
        }
        private DelegateCommand _signInCommand;
        public DelegateCommand SignInCommand
        {
            get
            {
                if (_signInCommand == null)
                {
                    _signInCommand = new DelegateCommand(SignIn, CanSignin);
                }

                return _signInCommand;
            }

        }

        private bool CanSignin(object obj)
        {
            if (!string.IsNullOrWhiteSpace(Username) )
                return true;
            else
                return false;
        }

        private async void SignIn(object obj)
        {

            //if (ProgressStarted != null)
            //{
            //    ProgressStarted("Signing In...", EventArgs.Empty);

            //}
            OnTaskStarted("Signing In...");
            LoginFailed = false;
            ErrorMessage = "";
            var passwordBox = obj as PasswordBox;
            Password = passwordBox.Password;
            UserDB user = new UserDB();
            user.Username = Username;

            
            var loginResponse = await HTTPHelper.LoginUser(new LoginModel { Password = Password, Username = Username });
            if (loginResponse==null)
            {
                LoginFailed = true;
                ErrorMessage = "Failed to Login,please contact administrator.";
                OnTaskCompleted("Failed to Login, please contact administrator.");
            }
            else
            {
                user.IsAdmin = loginResponse.Roles.Contains("Admin") ? true : false;
                
                Properties.Settings.Default.Username=Username;
                Properties.Settings.Default.Password=Password;
                Properties.Settings.Default.IsAdmin=user.IsAdmin;
                Properties.Settings.Default.SaveCredentials = SaveCredentials;
                Properties.Settings.Default.Save();

                //save a file to show user is logged in.
                string res = Path.GetTempPath();
                string json = JsonConvert.SerializeObject(user);
                File.WriteAllText(res+"wicrlogin.json", json);
                
            }
            if (OnLoggedIn != null)
            {
                OnLoggedIn(user, EventArgs.Empty);

            }
            OnTaskCompleted("");
        }
    }

}