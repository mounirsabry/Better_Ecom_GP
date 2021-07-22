using System;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Course
    {
        public string Course_code { get; set; }
        public string Department_code { get; set; }
        public string Course_name { get; set; }
        public int Academic_year { get; set; }
        public string Course_description { get; set; }
        public bool Is_read_only { get; set; }
        public bool Is_archived { get; set; }
        public Course(string courseCode, string departmentCode, string courseName, int academicYear, string courseDescription, bool isReadOnly, bool isArchived)
        {
            Course_code = courseCode;
            Department_code = departmentCode;
            Course_name = courseName;
            Academic_year = academicYear;
            Course_description = courseDescription;
            Is_read_only = isReadOnly;
            Is_archived = isArchived;
        }

        public Course() : this(null, null, null, -1, "", false, false) { }

        public Course(JsonElement jsonData)
        {
            JsonElement temp;
            if (jsonData.TryGetProperty(nameof(Course_code), out temp))
                Course_code = temp.GetString();
            else
                Course_code = null;

            if (jsonData.TryGetProperty(nameof(Department_code), out temp))
                Department_code = temp.GetString();
            else
                Department_code = null;

            if (jsonData.TryGetProperty(nameof(Course_name), out temp))
                Course_name = temp.GetString();
            else
                Course_name = null;

            if (jsonData.TryGetProperty(nameof(Academic_year), out temp))
                Academic_year = temp.GetInt32();
            else
                Academic_year = -1;

            if (jsonData.TryGetProperty(nameof(Course_description), out temp))
                Course_description = temp.GetString();
            else
                Course_description = "";

            if (jsonData.TryGetProperty(nameof(Is_read_only), out temp))
                Is_read_only = temp.GetBoolean();
            else
                Is_read_only = false;

            if (jsonData.TryGetProperty(nameof(Is_archived), out temp))
                Is_archived = temp.GetBoolean();
            else
                Is_archived = false;
        }

        public void Print()
        {
            Console.WriteLine("Course Info.");
            Console.WriteLine($"Course Code :{ Course_code }.");
            Console.WriteLine($"Department Code :{ Department_code }.");
            Console.WriteLine($"Course Name :{ Course_name }.");
            Console.WriteLine($"Academic Year :{ Academic_year }.");
            Console.WriteLine($"Course Description :{ Course_description }.");
            Console.WriteLine($"Is The Course Read Only :{ Is_read_only }.");
            Console.WriteLine($"Is The Course Archived :{ Is_archived }.");
        }
    }
}
