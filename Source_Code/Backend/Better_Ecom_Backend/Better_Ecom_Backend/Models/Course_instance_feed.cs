using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Models
{
    public class Course_instance_feed
    {
        public int Feed_id { get; set; }
        public int Course_instance_id { get; set; }
        public string Content { get; set; }
        public DateTime Insertion_date { get; set; }

        public Course_instance_feed(int feedID, int course_instance_id, string content, DateTime insertionDate)
        {
            Feed_id = feedID;
            Course_instance_id = course_instance_id;
            Content = content;
            Insertion_date = insertionDate;
        }

        public Course_instance_feed() : this(-1, -1, null, DateTime.MinValue) { }

        public Course_instance_feed(JsonElement jsonInput)
        {
            if (jsonInput.TryGetProperty(nameof(Feed_id), out JsonElement temp) && temp.TryGetInt32(out _))
                Feed_id = temp.GetInt32();
            else
                Feed_id = -1;

            if (jsonInput.TryGetProperty(nameof(Course_instance_id), out temp) && temp.TryGetInt32(out _))
                Course_instance_id = temp.GetInt32();

            else
                Course_instance_id = -1;

            if (jsonInput.TryGetProperty(nameof(Content), out temp) && temp.ValueKind == JsonValueKind.String)
                Content = temp.GetString();
            else
                Content = null;

            if (jsonInput.TryGetProperty(nameof(Insertion_date), out temp) && temp.TryGetDateTime(out _))
                Insertion_date = temp.GetDateTime();

        }
    }
}
