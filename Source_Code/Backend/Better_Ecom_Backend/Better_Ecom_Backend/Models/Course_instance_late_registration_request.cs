using Better_Ecom_Backend.Entities;
using System;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Course_instance_late_registration_request
    {
        public int Request_id { get; set; }
        public int Student_id { get; set; }
        public int Course_instance_id { get; set; }
        public DateTime Request_date { get; set; }
        public LateRegistrationRequestStatus Request_status{ get; set; }

        public Course_instance_late_registration_request(int requestID, int studentID, int courseInstanceID, DateTime requestDate, LateRegistrationRequestStatus requestStatus)
        {
            Request_id = requestID;
            Student_id = studentID;
            Course_instance_id = courseInstanceID;
            Request_date = requestDate;
            Request_status = requestStatus;
        }

        public Course_instance_late_registration_request() : this(-1, -1, -1, DateTime.MinValue, LateRegistrationRequestStatus.Pending_Accept) { }

        public Course_instance_late_registration_request(JsonElement jsonInput)
        {
            JsonElement temp;
            if (jsonInput.TryGetProperty(nameof(Request_id), out temp))
                Request_id = temp.GetInt32();
            else
                Request_id = -1;

            if (jsonInput.TryGetProperty(nameof(Student_id), out temp))
                Student_id = temp.GetInt32();
            else
                Student_id = -1;

            if (jsonInput.TryGetProperty(nameof(Course_instance_id), out temp))
                Course_instance_id = temp.GetInt32();
            else
                Course_instance_id = -1;

            if (jsonInput.TryGetProperty(nameof(Request_date), out temp))
                Request_date = temp.GetDateTime();
            else
                Request_date = DateTime.MinValue;

            if (jsonInput.TryGetProperty(nameof(Request_status), out temp))
                Request_status = (LateRegistrationRequestStatus)temp.GetInt32();
            else
                Request_status = LateRegistrationRequestStatus.Pending_Accept;
        }

        public void Print()
        {
            Console.WriteLine("Late Registration Request Info.");
            Console.WriteLine($"Request ID :{ Request_id }.");
            Console.WriteLine($"Student ID :{ Student_id }.");
            Console.WriteLine($"Course Instance ID :{ Course_instance_id }.");
            Console.WriteLine($"Request Date :{ Request_date }.");
            Console.WriteLine($"Request Status :{ Request_status }.");
        }
    }
}
