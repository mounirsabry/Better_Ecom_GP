using Better_Ecom_Backend.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Models
{
    public class Student_attendance_item_attendance
    {
        public int Attendance_item_id { set; get; }
        public int Student_id { set; get; }
        public AttendanceStatus Attendance_status { get; set; }

        public Student_attendance_item_attendance(int Attendance_item_id, int Student_id, AttendanceStatus Attendance_status)
        {
            this.Attendance_item_id = Attendance_item_id;
            this.Student_id = Student_id;
            this.Attendance_status = Attendance_status;
        }

        public Student_attendance_item_attendance() : this(-1, -1, AttendanceStatus.Not_Specified) { }

    }
}
