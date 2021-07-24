using Better_Ecom_Backend.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Models
{
    public class Attendance_item
    {
        public int Item_id { get; set; }
        public int Course_instance_id { get; set; }
        public string Item_name { get; set; }
        public AttendanceType Attendance_type { get; set; }
        public DateTime Attendance_date { get; set; }


        public Attendance_item(int Item_id, int Course_instance_id, string Item_name, AttendanceType Attendance_type, DateTime Attendance_date)
        {
            this.Item_id = Item_id;
            this.Course_instance_id = Course_instance_id;
            this.Item_name = Item_name;
            this.Attendance_type = Attendance_type;
            this.Attendance_date = Attendance_date;
        }

        public Attendance_item() : this(-1, -1, null, AttendanceType.Section, DateTime.MinValue)
        {

        }

    }
}
