using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Contrib;
using System.Threading.Tasks;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Instructor : System_user
    {
        public int Instructor_id { get; set; }
        public string University { get; set; }
        public int Graduation_year { get; set; }
        public string Contact_info { get; set; }

        public Instructor() : base()
        {
            Instructor_id = -1;
            University = null;
            Graduation_year = -1;
            Contact_info = "";
        }

        public Instructor(JsonElement data) : base(data)
        {
            JsonElement temp;
            if (data.TryGetProperty("Instructor_ID", out temp))
                this.Instructor_id = temp.GetInt32();
            else
                this.Instructor_id = -1;

            if (data.TryGetProperty("University", out temp))
                this.University = temp.GetString();
            else
                this.University = null;

            if (data.TryGetProperty("Graduation_year", out temp))
                this.Graduation_year = temp.GetInt32();
            else
                this.Graduation_year = -1;

            if (data.TryGetProperty("Contact_info", out temp))
                this.Contact_info = temp.GetString();
            else
                this.Contact_info = "";
        }

        public override void Print()
        {
            base.Print();
            Console.WriteLine("Instructor Part Info.");
            Console.WriteLine($"Instructor ID :{ Instructor_id }.");
            Console.WriteLine($"University :{ University }.");
            Console.WriteLine($"Graduation Year :{ Graduation_year }.");
            Console.WriteLine($"Contact Info :{ Contact_info }.");
        }
    }
}

