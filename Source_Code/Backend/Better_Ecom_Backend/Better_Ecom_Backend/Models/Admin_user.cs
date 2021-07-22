using System;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Admin_user : System_user
    {
        public int Admin_user_id { get; set; }

        public Admin_user() : base()
        {
            Admin_user_id = -1;
        }

        public Admin_user(JsonElement data) : base(data)
        {
            JsonElement temp;
            if (data.TryGetProperty(nameof(Admin_user_id), out temp))
                this.Admin_user_id = temp.GetInt32();
            else
                this.Admin_user_id = -1;
        }

        public override void Print()
        {
            base.Print();
            Console.WriteLine("Admin Part Info.");
            Console.WriteLine($"Admin User ID :{ Admin_user_id }.");
        }
    }
}
