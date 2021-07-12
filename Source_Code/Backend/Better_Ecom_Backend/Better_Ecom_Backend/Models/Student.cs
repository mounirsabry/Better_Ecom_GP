﻿using System;
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
        public int Entrance_year { get; set; }
        public double GPA { get; set; }
        public string Department { get; set; }
        public int Academic_year { get; set; }

        public Student() { }

        public Student(JsonElement data) : base(data)
        {
            JsonElement temp;
            if (data.TryGetProperty("Student_id", out temp))
                this.Student_id = temp.GetInt32();
            else
                this.Student_id = -1;

            if (data.TryGetProperty("High_school_type", out temp))
                this.High_school_type = temp.GetString();

            if (data.TryGetProperty("Entrance_year", out temp))
                this.Entrance_year = temp.GetInt32();
            else
                this.Student_id = -1;

            if (data.TryGetProperty("GPA", out temp))
                this.GPA = temp.GetDouble();
            else
                this.GPA = 0.0;

            if (data.TryGetProperty("Department", out temp))
                this.Department = temp.GetString();

            if (data.TryGetProperty("Academic_year", out temp))
                this.Academic_year = temp.GetInt32();
            else
                this.Academic_year = -1;
        }

        public override void Print()
        {
            base.Print();
            Console.WriteLine("Student Part Info.");
            Console.WriteLine($"Student ID :{ Student_id }.");
            Console.WriteLine($"High School Type :{ High_school_type }.");
            Console.WriteLine($"Entrance Year :{ Entrance_year }.");
            Console.WriteLine($"GPA :{ GPA }.");
            Console.WriteLine($"Department :{ Department }.");
            Console.WriteLine($"Academic Year :{ Academic_year }.");
        }
    }
}
