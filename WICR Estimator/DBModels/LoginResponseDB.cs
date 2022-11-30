using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WICR_Estimator.DBModels
{
    public class LoginResponseDB
    {
        public string Token { get; set; }
        public string Expiration { get; set; }
        public List<string> Roles { get; set; }

    }
}
