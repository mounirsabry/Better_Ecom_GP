using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Models
{
    public class Course
    {
        public enum Course_Term
        {
            First,
            Second,
            Summer,
            Other,
        }
        public int Course_id { get; set; }
        public string Department_code { get; set; }
        public string Course_code { get; set; }
        public string Course_name { get; set; }
        public int Course_year { get; set; }
        public Course_Term Course_term { get; set; }
        public int Academic_year { get; set; }
        public int Credit_hours { get; set; }
        public string Course_description { get; set; }
        public bool Is_archived { get; set; }

        public Course(int course_id, string department_code, string course_code, string course_name, int course_year, Course_Term course_term, int academic_year, int credit_hours, string course_description, bool is_archived)
        {
            Course_id = course_id;
            Department_code = department_code;
            Course_code = course_code;
            Course_name = course_name;
            Course_year = course_year;
            Course_term = course_term;
            Academic_year = academic_year;
            Credit_hours = credit_hours;
            Course_description = course_description;
            Is_archived = is_archived;
        }

        public Course() : this(-1, null, null, null, -1, Course_Term.Other, -1, 0, "", false)
        { }

        public Course(JsonElement jsonData)
        {
            JsonElement temp;
            if (jsonData.TryGetProperty(nameof(Course_id), out temp))
                Course_id = temp.GetInt32();
            else
                Course_id = -1;

            if (jsonData.TryGetProperty(nameof(Department_code), out temp))
                Department_code = temp.GetString();
            else
                Department_code = null;

            if (jsonData.TryGetProperty(nameof(Course_code), out temp))
                Course_code = temp.GetString();
            else
                Course_code = null;

            if (jsonData.TryGetProperty(nameof(Course_name), out temp))
                Course_name = temp.GetString();
            else
                Course_name = null;

            if (jsonData.TryGetProperty(nameof(Course_year), out temp))
                Course_year = temp.GetInt32();
            else
                Course_year = -1;

            if (jsonData.TryGetProperty(nameof(Course_term), out temp))
                Course_term = (Course_Term)temp.GetInt32();
            else
                Course_term = Course_Term.Other;

            if (jsonData.TryGetProperty(nameof(Academic_year), out temp))
                Academic_year = temp.GetInt32();
            else
                Academic_year = -1;

            if (jsonData.TryGetProperty(nameof(Credit_hours), out temp))
                Credit_hours = temp.GetInt32();
            else
                Credit_hours = 0;

            if (jsonData.TryGetProperty(nameof(Course_description), out temp))
                Course_description = temp.GetString();
            else
                Course_description = "";

            if (jsonData.TryGetProperty(nameof(Is_archived), out temp))
                Is_archived = temp.GetBoolean();
            else
                Is_archived = false;
        }

        public void Print()
        {
            Console.WriteLine("Course Info.");
            Console.WriteLine($"Course ID :{ Course_id }.");
            Console.WriteLine($"Department Code :{ Department_code }.");
            Console.WriteLine($"Course Code :{ Course_code }.");
            Console.WriteLine($"Course Name :{ Course_name }.");
            Console.WriteLine($"Course Year :{ Course_year }.");
            Console.WriteLine($"Course Term :{ Course_term }.");
            Console.WriteLine($"Academic Year :{ Academic_year }.");
            Console.WriteLine($"Course Description :{ Course_description }.");
            Console.WriteLine($"Is Course Archived :{ Is_archived }.");
        }
    }
}
