using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{
    public class UserDB
    {
        //public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }

        public string UserType { get; set; }
    }

    public class UserWithRoles
    {
        //public string UserName { get; set; }
        //public string Email { get; set; }
        public UserDB User { get; set; }
        public List<string> Roles { get; set; }
    }
    public class LoginModel
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
    public class UpdateUserModel
    {
        public string Username { get; set; }
        public string NewEmail { get; set; }        
        public string NewPassword { get; set; }
    }

    public class UserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
