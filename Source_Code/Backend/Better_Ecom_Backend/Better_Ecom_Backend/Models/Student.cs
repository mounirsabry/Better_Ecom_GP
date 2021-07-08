﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Student : System_user
    {
        public Student(JsonElement data) : base(data)
        {
            
            this.Student_id = data.GetProperty("student_id").GetInt32();
            this.High_school_type = data.GetProperty("high_school_type").GetString();
            this.Entrance_year = data.GetProperty("entrance_year").GetString();
            this.GPA = data.GetProperty("gpa").GetDouble();
            this.Department = data.GetProperty("department").GetString();
            this.Academic_year = data.GetProperty("academic_year").GetInt32();


        }

        public override string GetUpdateQuery()
        {
            return @$"UPDATE student SET high_school_type = @High_school_type, entrance_year = @Entrance_year, gpa = @GPA, 
                    department = @Department, academic_year = @Academic_year where student_id = @Student_id";
        }

        public int Student_id { get; set; }
        public string High_school_type { get; set; }
        public string Entrance_year { get; set; }
        public double GPA { get; set; }
        public string Department { get; set; }
        public int Academic_year { get; set; }

    }
}
