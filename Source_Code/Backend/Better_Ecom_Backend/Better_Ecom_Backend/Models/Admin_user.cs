using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Admin : System_user
    {

        public Admin(JsonElement data) : base(data)
        {
            this.Admin_user_id = data.GetProperty("admin_user_id").GetInt32();
        }
        
        public int Admin_user_id { get; set; }

        public override string GetUpdateQuery()
        {
            throw new NotImplementedException();
        }
    }
}
