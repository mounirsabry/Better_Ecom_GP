using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Better_Ecom_Backend.Models
{
    public class System_user
    {
        public int system_user_id { get; set; }
        public string user_password { get; set; }
        public string full_name { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string phone_number { get; set; }
        public string mobile_number { get; set; }
        public string nationality { get; set; }
        public string national_id { get; set; }
        public DateTime birth_date { get; set; }
        public string gender { get; set; }
        public string additional_info { get; set; }
    }
}
