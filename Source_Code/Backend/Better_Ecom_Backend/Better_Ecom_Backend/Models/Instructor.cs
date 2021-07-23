using System;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Instructor : System_user
    {
        public int Instructor_id { get; set; }
        public string Department_code { get; set; }
        public string University { get; set; }
        public int Graduation_year { get; set; }
        public string Contact_info { get; set; }

        public Instructor() : base()
        {
            Instructor_id = -1;
            Department_code = null;
            University = null;
            Graduation_year = -1;
            Contact_info = "";
        }

        public Instructor(JsonElement data) : base(data)
        {
            JsonElement temp;
            if (data.TryGetProperty(nameof(Instructor_id), out temp))
                this.Instructor_id = temp.GetInt32();
            else
                this.Instructor_id = -1;

            if (data.TryGetProperty(nameof(Department_code), out temp))
                this.Department_code = temp.GetString();
            else
                this.Department_code = null;

            if (data.TryGetProperty(nameof(University), out temp))
                this.University = temp.GetString();
            else
                this.University = null;

            if (data.TryGetProperty(nameof(Graduation_year), out temp))
                this.Graduation_year = temp.GetInt32();
            else
                this.Graduation_year = -1;

            if (data.TryGetProperty(nameof(Contact_info), out temp))
                this.Contact_info = temp.GetString();
            else
                this.Contact_info = "";
        }

        public override void Print()
        {
            base.Print();
            Console.WriteLine("Instructor Part Info.");
            Console.WriteLine($"Instructor ID :{ Instructor_id }.");
            Console.WriteLine($"Department Code :{ Department_code }.");
            Console.WriteLine($"University :{ University }.");
            Console.WriteLine($"Graduation Year :{ Graduation_year }.");
            Console.WriteLine($"Contact Info :{ Contact_info }.");
        }
    }
}

