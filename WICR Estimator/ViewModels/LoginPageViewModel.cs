using System;

namespace WICR_Estimator.ViewModels
{
    internal class LoginPageViewModel :BaseViewModel, IPageViewModel
    {
        public string Name => "Login Page";

        //private string _userName;
        //public string SelectedProject
        //{
        //    get
        //    {
        //        return _userName;
        //    }
        //    set
        //    {
        //        if (_userName != value)
        //        {
        //            _userName = value;

        //            OnPropertyChanged("UserName");
        //        }
        //    }
        //}
        public string Username { get; set; }
        string string Password { get; set; }
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
            return !string.IsNullOrEmpty(Username);
        }

        private void SignIn(object obj)
        {
            
        }
    }

}