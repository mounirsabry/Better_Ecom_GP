using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Student : System_user
    {
        public int Student_id { get; set; }
        public string High_school_type { get; set; }
        public string Entrance_year { get; set; }
        public double GPA { get; set; }
        public string Department { get; set; }
        public int Academic_year { get; set; }

        public Student() { }

        public Student(JsonElement data) : base(data)
        {
            JsonElement temp;
            if(data.TryGetProperty("Student_id",out temp))
                this.Student_id = temp.GetInt32();

            if (data.TryGetProperty("Entrance_year", out temp))
                this.Entrance_year = temp.GetString();

            if (data.TryGetProperty("High_school_type", out temp))
                this.High_school_type = temp.GetString();

            if (data.TryGetProperty("GPA", out temp))
                this.GPA = temp.GetDouble();

            if (data.TryGetProperty("Department", out temp))
                this.Department = temp.GetString();

            if (data.TryGetProperty("Academic_year", out temp))
                this.Academic_year = temp.GetInt32();
        }

    }
}
