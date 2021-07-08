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
        public System_user()
        {

        }

        public System_user(JsonElement data)
        {
            this.System_user_id = data.GetProperty("system_user_id").GetInt32();
            this.Full_name = data.GetProperty("full_name").GetString();
            this.Email = data.GetProperty("email").GetString();
            this.Address = data.GetProperty("address").GetString();
            this.Phone_number = data.GetProperty("phone_number").GetString();
            this.Mobile_number = data.GetProperty("mobile_number").GetString();
            this.Nationality = data.GetProperty("nationality").GetString();
            this.National_id = data.GetProperty("national_id").GetString();
            this.Birth_date = data.GetProperty("birth_date").GetDateTime();
            this.Gender = data.GetProperty("gender").GetString();
            this.Additional_info = data.GetProperty("additional_info").GetString();
        }

        public string GetBaseUpdateQuery()
        {
            return @$"UPDATE system_user SET full_name = @Full_name, email = 'g.r33r@hotmail.com', address = @Address, phone_number = @Phone_number, 
                mobile_number = @Mobile_number, nationality = @Nationality, national_id = @National_id, Birth_date = @Birth_date, gender = @Gender,
                additional_info = @Additional_info  where system_user_id = @System_user_id";
        }

        public abstract string GetUpdateQuery();

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



    }
}
