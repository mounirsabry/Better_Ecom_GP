using Better_Ecom_Backend.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Models
{
    public class StudentAttendanceItemAndInfo
    {
        public int Item_id { get; set; }
        public int Course_instance_id { get; set; }
        public string Item_name { get; set; }
        public AttendanceType Attendance_type { get; set; }
        public DateTime Attendance_date { get; set; }
        public int Student_id { set; get; }
        public AttendanceStatus Attendance_status { get; set; }
    }
}
