using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusLearnApplication.Models
{
    internal class Login
    {
        public int loginID { get; set; }
        public string email { get; set; }
        public string passwordHash { get; set; }
        public string role { get; set; }


        public void updatePassword()
        {
            //logic
        }

        public void recieveReport()
        {
            //logic
        }

    }
}
