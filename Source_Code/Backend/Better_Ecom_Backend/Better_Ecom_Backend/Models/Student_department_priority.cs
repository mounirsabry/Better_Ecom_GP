using System;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Student_department_priority
    {
        public int Student_id { get; set; }
        public string Department_code { get; set; }
        public int Priority { get; set; }

        //Priority represents the priority of a specific department for a specific student.
        //The lower the priority value the higher the priority, for example 1 will be the highest priority.
        //5 will be lower than 4, 3, 2, 1.
        //-1 indicates unassigned priority.

        public Student_department_priority(int studentID, string departmentCode, int priority)
        {
            Student_id = studentID;
            Department_code = departmentCode;
            Priority = priority;
        }

        public Student_department_priority() : this(-1, null, -1)
        { }

        public Student_department_priority(JsonElement data)
        {
            JsonElement temp;
            if (data.TryGetProperty(nameof(Student_id), out temp))
                Student_id = temp.GetInt32();
            else
                Student_id = -1;

            if (data.TryGetProperty(nameof(Department_code), out temp))
                Department_code = temp.GetString();
            else
                Department_code = null;

            if (data.TryGetProperty(nameof(Priority), out temp))
                Priority = temp.GetInt32();
            else
                Priority = -1;
        }

        public void Print()
        {
            Console.WriteLine($"Student { Student_id} wants to join { Department_code } with priority { Priority }.");
        }
    }
}
