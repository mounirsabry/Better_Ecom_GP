using Better_Ecom_Backend.Entities;
using System;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Student_course_instance_registration
    {
        public int Registration_id { get; set; }
        public int Student_id { get; set; }
        public int Course_instance_id { get; set; }
        public DateTime Registration_date { get; set; }
        public StudentCourseInstanceRegistrationStatus Student_course_instance_status { get; set; }

        public Student_course_instance_registration(int registrationID, int studentID, int courseInstanceID, DateTime registrationDate, StudentCourseInstanceRegistrationStatus studentCourseInstanceStatus)
        {
            Registration_id = registrationID;
            Student_id = studentID;
            Course_instance_id = courseInstanceID;
            Registration_date = registrationDate;
            Student_course_instance_status = studentCourseInstanceStatus;
        }

        public Student_course_instance_registration() : this(-1, -1, -1, DateTime.MinValue, StudentCourseInstanceRegistrationStatus.Undertaking) { }

        public Student_course_instance_registration(JsonElement jsonInput)
        {
            JsonElement temp;
            if (jsonInput.TryGetProperty(nameof(Registration_id), out temp))
                Registration_id = temp.GetInt32();
            else
                Registration_id = -1;

            if (jsonInput.TryGetProperty(nameof(Student_id), out temp))
                Student_id = temp.GetInt32();
            else
                Student_id = -1;

            if (jsonInput.TryGetProperty(nameof(Course_instance_id), out temp))
                Course_instance_id = temp.GetInt32();
            else
                Course_instance_id = -1;

            if (jsonInput.TryGetProperty(nameof(Registration_date), out temp))
                Registration_date = temp.GetDateTime();
            else
                Registration_date = DateTime.MinValue;

            if (jsonInput.TryGetProperty(nameof(Student_course_instance_status), out temp))
                Student_course_instance_status = (StudentCourseInstanceRegistrationStatus)temp.GetInt32();
            else
                Student_course_instance_status = StudentCourseInstanceRegistrationStatus.Undertaking;
        }

        public void Print()
        {
            Console.WriteLine("Student Course Instance Registration Info.");
            Console.WriteLine($"Registration ID :{ Registration_id }.");
            Console.WriteLine($"Student ID :{ Student_id }.");
            Console.WriteLine($"Course Instance ID :{ Course_instance_id }.");
            Console.WriteLine($"Registration Date :{ Registration_date }.");
            Console.WriteLine($"Student Course Instance Status :{ Student_course_instance_status }.");
        }
    }
}
