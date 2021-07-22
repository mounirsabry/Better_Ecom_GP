using System;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{


    public class Student : System_user
    {

        public int Student_id { get; set; }
        public string Department_code { get; set; }
        public string High_school_type { get; set; }
        public int Entrance_year { get; set; }
        public double GPA { get; set; }
        public int Academic_year { get; set; }

        public Student() : base()
        {
            Student_id = -1;
            Department_code = null;
            High_school_type = null;
            Entrance_year = -1;
            GPA = 0.0;
            Academic_year = -1;
        }

        public Student(JsonElement data) : base(data)
        {
            JsonElement temp;
            if (data.TryGetProperty(nameof(Student_id), out temp))
                this.Student_id = temp.GetInt32();
            else
                this.Student_id = -1;

            if (data.TryGetProperty(nameof(Department_code), out temp))
                this.Department_code = temp.GetString();
            else
                this.Department_code = null;

            if (data.TryGetProperty(nameof(High_school_type), out temp))
                this.High_school_type = temp.GetString();
            else
                this.High_school_type = null;

            if (data.TryGetProperty(nameof(Entrance_year), out temp))
                this.Entrance_year = temp.GetInt32();
            else
                this.Student_id = -1;

            if (data.TryGetProperty(nameof(GPA), out temp))
                this.GPA = temp.GetDouble();
            else
                this.GPA = 0.0;

            if (data.TryGetProperty(nameof(Academic_year), out temp))
                this.Academic_year = temp.GetInt32();
            else
                this.Academic_year = -1;
        }

        public override void Print()
        {
            base.Print();
            Console.WriteLine("Student Part Info.");
            Console.WriteLine($"Student ID :{ Student_id }.");
            Console.WriteLine($"Department :{ Department_code }.");
            Console.WriteLine($"High School Type :{ High_school_type }.");
            Console.WriteLine($"Entrance Year :{ Entrance_year }.");
            Console.WriteLine($"GPA :{ GPA }.");
            Console.WriteLine($"Academic Year :{ Academic_year }.");
        }
    }
}
