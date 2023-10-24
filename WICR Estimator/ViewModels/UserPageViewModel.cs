using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WICR_Estimator.DBModels;
using WICR_Estimator.Services;

namespace WICR_Estimator.ViewModels
{
    public class UserPageViewModel : BaseViewModel, IPageViewModel
    {
        public UserPageViewModel()
        {           
            UserType = "User";
            FetchUsers();
        }

        private async void FetchUsers()
        {
            ActiveUsers = await HTTPHelper.GetAllUsers();
            ActiveUsers.ToList().ForEach(c => c.User.UserType = c.Roles.Contains("Admin")?"Admin":"User");
            
            OnPropertyChanged("ActiveUsers");
        }

        public IList<UserWithRoles> ActiveUsers { get; set; }
        public bool IsNewUser { get; set; }
        public string Header { get; set; }
        public string ButtonText { get; set; }
        public string UpdateStatusMessage { get; set; }
        public string AddStatusMessage { get; set; }
        
        private UserWithRoles _userWithRoles ;
        public UserWithRoles SelectedUser
        {
            get
            {
                return _userWithRoles;
            }
            set
            {
                if (_userWithRoles != value)
                {
                    _userWithRoles = value;
                    if (value == null)
                    {
                        Header = "Add User";
                        ButtonText = "Add user";
                        IsNewUser = true;
                        Email = "";
                        UserName = "";
                        UserType = "User";
                    }
                    else
                    {
                        Email = _userWithRoles.User.Email;
                        UserName = _userWithRoles.User.Username;
                        UserType = _userWithRoles.User.UserType;
                        Header = "Edit User";
                        ButtonText = "Save user";
                        IsNewUser = false;
                    }
                    UpdateStatusMessage = "";
                    OnPropertyChanged("UserName");
                    OnPropertyChanged("Header");
                    OnPropertyChanged("ButtonText");
                    OnPropertyChanged("UpdateStatusMessage");
                }
            }
        }

        
        private DelegateCommand _deleteUserCommand;
        public DelegateCommand DeleteUserCommand
        {
            get
            {
                if (_deleteUserCommand == null)
                {
                    _deleteUserCommand = new DelegateCommand(DeleteUser, CanDeleteUser);
                }

                return _deleteUserCommand;
            }

        }
        private DelegateCommand _addUserCommand;
        public DelegateCommand AddUserCommand
        {
            get
            {
                if (_addUserCommand == null)
                {
                    _addUserCommand = new DelegateCommand(AddUser, CanAddUser);
                }

                return _addUserCommand;
            }

        }

        private bool CanAddUser(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;
            if (NewUserName!=null && NewEmail!=null && password.Length!=0)
            {
                return true;
            }
            return false;
        }

        private async void AddUser(Object parameter)
        {
            SuccessResponse response;
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;
            UserModel userModel = new UserModel() { Username =NewUserName, Email = NewEmail, Password = password };
            if (UserType == "Admin")
            {
                response = await HTTPHelper.AddAdminUser(userModel);
            }
            else
            {
                response = await HTTPHelper.AddUser(userModel);
            }

            if (response == null)
            {
                AddStatusMessage = "Failed to add the user";
                
            }
            else
                AddStatusMessage = response.Message;
            OnPropertyChanged("AddStatusMessage");
        }
        private string _newUserName;
        public string NewUserName
        {
            get
            {
                return _newUserName;
            }
            set
            {
                if (_newUserName != value)
                {
                    _newUserName = value;
                    OnPropertyChanged("NewUserName");
                }
            }
        }
        private string _newEmail;
        public string NewEmail
        {
            get
            {
                return _newEmail;
            }
            set
            {
                if (_newEmail != value)
                {
                    _newEmail = value;
                    OnPropertyChanged("NewEmail");
                }
            }
        }
        public bool IsAdmin { get; set; }
        private async void DeleteUser(object obj)
        {
            var user = obj as UserWithRoles;
            if (user!=null)
            {
                var response = await HTTPHelper.DeleteUser(user.User.Username);
                if (response == null)
                {
                    UpdateStatusMessage = "Failed to delete the user";
                    return;
                }
                UpdateStatusMessage = response.Message;
            }            
        }

        private bool CanDeleteUser(object obj)
        {
            var user = obj as UserWithRoles;
            if (user!=null)
            {
                if (user.User.Username == "admin")
                    return false;
            }
            return true;
        }

        private DelegateCommand _saveUserCommand;
        public DelegateCommand SaveUserCommand
        {
            get
            {
                if (_saveUserCommand == null)
                {
                    _saveUserCommand = new DelegateCommand(SaveUser, CanSaveUser);
                }

                return _saveUserCommand;
            }

        }

        private bool CanSaveUser(object obj)
        {
            if (UserName!=null && Email!=null)
            {
                return true;
            }
            else
                return false;
        }

        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged("UserName");
                }
            }
        }
        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged("Email");
                }
            }
        }
        private string _userType;
        public string UserType
        {
            get
            {
                return _userType;
            }
            set
            {
                if (_userType != value)
                {
                    _userType = value;
                    OnPropertyChanged("UserType");
                }
            }
        }

        public string Name => "Manage Users";

        private async void SaveUser(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;
             
            UpdateUserModel userModel = new UpdateUserModel() { NewEmail = Email, NewPassword = password, Username = UserName };
            SuccessResponse response = await HTTPHelper.UpdateUser(userModel, UserType);
            if (response == null)
            {
                UpdateStatusMessage = "Failed to update the user";

            }
            else
            {
                UpdateStatusMessage = response.Message;
            }
            
           
            OnPropertyChanged("UpdateStatusMessage");
        }
    }
}
