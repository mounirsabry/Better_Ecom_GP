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
            this.Instructor_id = data.GetProperty("instructor_id").GetInt32();
            this.University = data.GetProperty("university").GetString();
            this.Graduation_year = data.GetProperty("graduation_year").GetInt32();
            this.Contact_info = data.GetProperty("contact_info").GetString();
        }

        public override string GetUpdateQuery()
        {
            return @$"UPDATE instructor SET university = @University, graduation_year = @Graduation_year, contact_info = @Contact_info where instructor_id = @Instructor_id";
        }
    }
}
