using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class General_feed
    {
        public int Feed_id { get; set; }
        public string Content { get; set; }
        public DateTime Insertion_date { get; set; }

        public General_feed(int feedID, string content, DateTime insertionDate)
        {
            Feed_id = feedID;
            Content = content;
            Insertion_date = insertionDate;
        }

        public General_feed() : this(-1, null, DateTime.MinValue) { }

        public General_feed(JsonElement jsonInput)
        {
            if (jsonInput.TryGetProperty(nameof(Feed_id), out JsonElement temp) && temp.TryGetInt32(out _))
                Feed_id = temp.GetProperty(nameof(Feed_id)).GetInt32();
            else
                Feed_id = -1;

            if (jsonInput.TryGetProperty(nameof(Content), out temp) && temp.ValueKind == JsonValueKind.String)
                Content = temp.GetProperty(nameof(Content)).GetString();
            else
                Content = null;

            if (jsonInput.TryGetProperty(nameof(Insertion_date), out temp) && temp.TryGetDateTime(out _))
                Insertion_date = temp.GetProperty(nameof(Insertion_date)).GetDateTime();
            else
                Insertion_date = DateTime.MinValue;
        }

        public void Print()
        {
            Console.WriteLine("General Feed Info.");
            Console.WriteLine($"Feed ID :{ Feed_id }.");
            Console.WriteLine($"Content :{ Content }.");
            Console.WriteLine($"Insertion Date :{ Insertion_date }.");
        }
    }
}
