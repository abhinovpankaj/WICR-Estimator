using System;
using System.Security;
using WICR_Estimator.DBModels;

namespace WICR_Estimator.ViewModels
{
    internal class LoginPageViewModel :BaseViewModel, IPageViewModel
    {

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
            if (!string.IsNullOrWhiteSpace(Username) && PasswordSecureString != null && PasswordSecureString.Length > 0)
                return true;
            else
                return false;
        }

        private void SignIn(object obj)
        {
            LoginFailed = false;
            UserDB user = new UserDB();
            user.Username = Username;
            
            if (Username.ToLower() == "admin")
            {
                
                if (OnLoggedIn != null)
                {
                    user.IsAdmin = true;
                    OnLoggedIn(user, EventArgs.Empty);
                }
            }
            else
            {
                user.IsAdmin = false;
                if (OnLoggedIn != null)
                {
                    OnLoggedIn(user, EventArgs.Empty);
                }
            }         
 
        }
    }

}