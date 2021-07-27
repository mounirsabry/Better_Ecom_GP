using Better_Ecom_Backend.Entities;
using System;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Course_instance
    {
        public int Instance_id { get; set; }
        public string Course_code { get; set; }
        public int Course_year { get; set; }
        public Term Course_term { get; set; }
        public int Credit_hours { get; set; }

        public Course_instance(int instanceID, string courseCode, int courseYear, Term courseTerm, int creditHours)
        {
            Instance_id = instanceID;
            Course_code = courseCode;
            Course_year = courseYear;
            Course_term = courseTerm;
            Credit_hours = creditHours;
        }

        public Course_instance() : this(-1, null, -1, Term.Other, 0) { }

        public Course_instance(JsonElement jsonData)
        {
            JsonElement temp;
            if (jsonData.TryGetProperty(nameof(Instance_id), out temp))
                Instance_id = temp.GetInt32();
            else
                Instance_id = -1;

            if (jsonData.TryGetProperty(nameof(Course_code), out temp))
                Course_code = temp.GetString();
            else
                Course_code = null;

            if (jsonData.TryGetProperty(nameof(Course_year), out temp))
                Course_year = temp.GetInt32();
            else
                Course_year = -1;

            if (jsonData.TryGetProperty(nameof(Course_term), out temp))
                Course_term = (Term)temp.GetInt32() + 1;
            else
                Course_term = Term.Other;

            if (jsonData.TryGetProperty(nameof(Credit_hours), out temp))
                Credit_hours = temp.GetInt32();
            else
                Credit_hours = 0;
        }

        public void Print()
        {
            Console.WriteLine("Course Instance Info.");
            Console.WriteLine($"Instance ID :{ Instance_id }.");
            Console.WriteLine($"Course Code :{ Course_code }.");
            Console.WriteLine($"Course Year :{ Course_year }.");
            Console.WriteLine($"Course Term :{ Course_term }.");
            Console.WriteLine($"Course Credit Hours :{ Credit_hours }.");
        }
    }
}
