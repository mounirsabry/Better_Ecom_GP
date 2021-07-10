using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Admin_user : System_user
    {
        public int Admin_user_id { get; set; }

        public Admin_user() { }

        public Admin_user(JsonElement data) : base(data)
        {
            JsonElement temp;
            if (data.TryGetProperty("Admin_user_id", out temp))
            {
                this.Admin_user_id = temp.GetInt32();
            }
            else
            {
                this.Admin_user_id = -1;
            }
        }
    }
}
