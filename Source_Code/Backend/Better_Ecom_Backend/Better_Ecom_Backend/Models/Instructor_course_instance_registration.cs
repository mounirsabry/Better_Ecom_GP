using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Instructor_course_instance_registration
    {
        public int Registration_id { get; set; }
        public int Instructor_id { get; set; }
        public int Course_instance_id { get; set; }
        public DateTime Registration_date { get; set; }

        public Instructor_course_instance_registration(int registrationID, int instructorID, int courseInstanceID, DateTime registrationDate)
        {
            Registration_id = registrationID;
            Instructor_id = instructorID;
            Course_instance_id = courseInstanceID;
            Registration_date = registrationDate;
        }

        public Instructor_course_instance_registration() : this(-1, -1, -1, DateTime.MinValue) { }

        public Instructor_course_instance_registration(JsonElement jsonInput)
        {
            if (jsonInput.TryGetProperty(nameof(Registration_id), out JsonElement temp) && temp.TryGetInt32(out _))
                Registration_id = temp.GetProperty(nameof(Registration_id)).GetInt32();
            else
                Registration_id = -1;
        }

    }
}
