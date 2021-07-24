using Better_Ecom_Backend.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Models
{
    public class GpaData
    {
        public double Credit_hours { get; set; }
        public StudentCourseInstanceGrade Student_course_instance_grade { get; set; }

        public GpaData (int creditHours, StudentCourseInstanceGrade grade)
        {
            this.Credit_hours = creditHours;
            this.Student_course_instance_grade = grade;
        }
    }
}
