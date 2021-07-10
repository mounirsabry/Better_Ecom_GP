using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Contrib;
using System.Threading.Tasks;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Instructor: System_user
    {
        public int Instructor_id { get; set; }
        public string University { get; set; }
        public int Graduation_year { get; set; }
        public string Contact_info { get; set; }

        public Instructor() { }

        public Instructor(JsonElement data) : base(data)
        {
            JsonElement temp;
            if (data.TryGetProperty("Instructor_ID", out temp))
                this.Instructor_id = temp.GetInt32();

            if (data.TryGetProperty("University", out temp))
                this.University = temp.GetString();

            if (data.TryGetProperty("Graduation_year", out temp))
                this.Graduation_year = temp.GetInt32();

            if (data.TryGetProperty("Contact_info", out temp))
                this.Contact_info = temp.GetString();
        }
    }
}
