using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Models
{
    public class Course_instance
    {
        public enum Course_Term
        {
            First,
            Second,
            Summer,
            Other,
        }
        public string Course_code { get; set; }
        public int Course_year { get; set; }
        public Course_Term Course_term { get; set; }
        public int Credit_hours { get; set; }

        public Course_instance(string courseCode, int courseYear, Course_Term courseTerm, int creditHours)
        {
            Course_code = courseCode;
            Course_year = courseYear;
            Course_term = courseTerm;
            Credit_hours = creditHours;
        }

        public Course_instance() : this(null, -1, Course_Term.Other, 0) { }

        public Course_instance(JsonElement jsonData)
        {
            JsonElement temp;
            if (jsonData.TryGetProperty(nameof(Course_code), out temp))
                Course_code = temp.GetString();
            else
                Course_code = null;

            if (jsonData.TryGetProperty(nameof(Course_year), out temp))
                Course_year = temp.GetInt32();
            else
                Course_year = -1;

            if (jsonData.TryGetProperty(nameof(Course_term), out temp))
                Course_term = (Course_Term)temp.GetInt32();
            else
                Course_term = Course_Term.Other;

            if (jsonData.TryGetProperty(nameof(Credit_hours), out temp))
                Credit_hours = temp.GetInt32();
            else
                Credit_hours = 0;
        }

        public void Print()
        {
            Console.WriteLine("Course Instance Info.");
            Console.WriteLine($"Course Code :{ Course_code }.");
            Console.WriteLine($"Course Year :{ Course_year }.");
            Console.WriteLine($"Course Term :{ Course_term }.");
            Console.WriteLine($"Course Credit Hours :{ Credit_hours }.");
        }
    }
}
