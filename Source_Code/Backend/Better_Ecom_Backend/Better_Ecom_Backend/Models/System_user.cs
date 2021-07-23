using System;
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
            System_user_id = -1;
            User_password = null;
            Full_name = null;
            Email = null;
            Address = null;
            Phone_number = null;
            Mobile_number = null;
            Nationality = null;
            National_id = null;
            Birth_date = DateTime.MinValue;
            Gender = null;
            Additional_info = "";
        }

        public System_user(JsonElement data)
        {
            JsonElement temp;
            if (data.TryGetProperty(nameof(System_user_id), out temp))
                this.System_user_id = temp.GetInt32();
            else
                this.System_user_id = -1;

            //This condition may be never by true, but I checked it anyways.
            //Maybe be useful in other function other than register new uesr.
            if (data.TryGetProperty(nameof(User_password), out temp))
                this.User_password = temp.GetString();
            else
                this.User_password = null;

            if (data.TryGetProperty(nameof(Full_name), out temp))
                this.Full_name = temp.GetString();
            else
                this.Full_name = null;

            if (data.TryGetProperty(nameof(Email), out temp))
                this.Email = temp.GetString();
            else
                this.Email = null;

            if (data.TryGetProperty(nameof(Address), out temp))
                this.Address = temp.GetString();
            else
                this.Address = null;

            if (data.TryGetProperty(nameof(Phone_number), out temp))
                this.Phone_number = temp.GetString();
            else
                this.Phone_number = null;

            if (data.TryGetProperty(nameof(Mobile_number), out temp))
                this.Mobile_number = temp.GetString();
            else
                this.Mobile_number = null;

            if (data.TryGetProperty(nameof(Nationality), out temp))
                this.Nationality = temp.GetString();
            else
                this.Nationality = null;

            if (data.TryGetProperty(nameof(National_id), out temp))
                this.National_id = temp.GetString();
            else
                this.National_id = null;

            if (data.TryGetProperty(nameof(Birth_date), out temp))
                this.Birth_date = temp.GetDateTime();
            else
                this.Birth_date = DateTime.MinValue;

            if (data.TryGetProperty(nameof(Gender), out temp))
                this.Gender = temp.GetString();
            else
                this.Gender = null;

            if (data.TryGetProperty(nameof(Additional_info), out temp))
                this.Additional_info = temp.GetString();
            else
                this.Additional_info = "";
        }

        public virtual void Print()
        {
            Console.WriteLine("System User Info.");
            Console.WriteLine($"System User ID :{ System_user_id }.");
            Console.WriteLine($"User Password :{ User_password }.");
            Console.WriteLine($"Full Name :{ Full_name }.");
            Console.WriteLine($"Email :{ Email }.");
            Console.WriteLine($"Address :{ Address }.");
            Console.WriteLine($"Phone Number :{ Phone_number }.");
            Console.WriteLine($"Mobile Number :{ Mobile_number }.");
            Console.WriteLine($"Nationality :{ Nationality }.");
            Console.WriteLine($"Nationl ID :{ National_id }.");
            Console.WriteLine($"Birth Date :{ Birth_date }.");
            Console.WriteLine($"Gender :{ Gender }.");
            Console.WriteLine($"Additional Info :{ Additional_info }.");
        }
    }
}
