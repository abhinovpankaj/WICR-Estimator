using System;
using System.Security;
using System.Windows.Controls;
using WICR_Estimator.DBModels;
using WICR_Estimator.Services;

namespace WICR_Estimator.ViewModels
{
    internal class LoginPageViewModel :BaseViewModel, IPageViewModel
    {

        public LoginPageViewModel()
        {
            Username = Properties.Settings.Default.Username;
            Password= Properties.Settings.Default.Password;
        }
        private string Password { get; set; }
        public static event EventHandler OnLoggedIn;
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
            LoginFailed = false;
            var passwordBox = obj as PasswordBox;
            Password = passwordBox.Password;
            UserDB user = new UserDB();
            user.Username = Username;
            

            var loginResponse = await HTTPHelper.LoginUser(new LoginModel { Password = Password, Username = Username });
            if (loginResponse==null)
            {
                LoginFailed = true;
                ErrorMessage = "Failed to Login,please contact administrator.";
            }
            else
            {
                user.IsAdmin = loginResponse.Roles.Contains("Admin") ? true : false;
                if (OnLoggedIn != null)
                {
                    OnLoggedIn(user, EventArgs.Empty);

                }
                Properties.Settings.Default.Username=Username;
                Properties.Settings.Default.Password=Password;

                Properties.Settings.Default.Save();
            }
        }
    }

}