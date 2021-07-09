using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib;
using System.Text.Json;
namespace Better_Ecom_Backend.Models
{
    public abstract class System_user
    {
        public int System_user_id { get; set; }
        public string User_password { get; set; }
        public string Full_name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone_number { get; set; }
        public string Mobile_number { get; set; }
        public string Nationality { get; set; }
        public string National_id { get; set; }
        public DateTime Birth_date { get; set; }
        public string Gender { get; set; }
        public string Additional_info { get; set; }

        public System_user()
        {

        }

        public System_user(JsonElement data)
        {
            JsonElement temp;
            if (data.TryGetProperty("System_user_id", out temp))
                this.System_user_id = temp.GetInt32();

            if (data.TryGetProperty("Full_name", out temp))
                this.Full_name = temp.GetString();

            if (data.TryGetProperty("Email", out temp))
                this.Email = temp.GetString();

            if (data.TryGetProperty("Address", out temp))
                this.Address = temp.GetString();

            if (data.TryGetProperty("Phone_number", out temp))
                this.Phone_number = temp.GetString();

            if (data.TryGetProperty("Mobile_number", out temp))

                this.Mobile_number = temp.GetString();

            if (data.TryGetProperty("Nationality", out temp))
                this.Nationality = temp.GetString();

            if (data.TryGetProperty("National_id", out temp))
                this.National_id = temp.GetString();

            if (data.TryGetProperty("Birth_date", out temp))
                this.Birth_date = temp.GetDateTime();

            if (data.TryGetProperty("Gender", out temp))
                this.Gender = temp.GetString();

            if (data.TryGetProperty("Additional_info", out temp))
                this.Additional_info = temp.GetString();
        }




    }
}
