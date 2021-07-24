using Better_Ecom_Backend.Entities;
using Better_Ecom_Backend.Models;
using System.Collections.Generic;

namespace Better_Ecom_Backend.Helpers
{
    public class EnumFunctions
    {
        public static List<LateRegistrationRequestStatus> GetLateRegistrationRequestStausList()
        {
            List<LateRegistrationRequestStatus> status = new()
            {
                LateRegistrationRequestStatus.Pending_Accept,
                LateRegistrationRequestStatus.Accepted,
                LateRegistrationRequestStatus.Rejected
            };
            return status;
        }

        public static List<StudentCourseInstanceRegistrationStatus> GetStudentCourseInstanceRegistrationStatusList()
        {
            List<StudentCourseInstanceRegistrationStatus> status = new()
            {
                StudentCourseInstanceRegistrationStatus.Undertaking,
                StudentCourseInstanceRegistrationStatus.Passed,
                StudentCourseInstanceRegistrationStatus.Failed
            };
            return status;
        }

        public static List<StudentCourseInstanceGrade> GetStudentCourseInstanceGradeList()
        {
            List<StudentCourseInstanceGrade> grades = new()
            {
                StudentCourseInstanceGrade.APlus,
                StudentCourseInstanceGrade.A,
                StudentCourseInstanceGrade.BPlus,
                StudentCourseInstanceGrade.B,
                StudentCourseInstanceGrade.CPlus,
                StudentCourseInstanceGrade.C,
                StudentCourseInstanceGrade.DPlus,
                StudentCourseInstanceGrade.D,
                StudentCourseInstanceGrade.F,
                StudentCourseInstanceGrade.Not_Specified
            };
            return grades;
        }

        public static double GetStudentCourseInstanceGradePoints(StudentCourseInstanceGrade grade)
        {
            switch (grade)
            {
                case StudentCourseInstanceGrade.APlus:
                    return 4.0;
                case StudentCourseInstanceGrade.A:
                    return 3.7;
                case StudentCourseInstanceGrade.BPlus:
                    return 3.3;
                case StudentCourseInstanceGrade.B:
                    return 3.0;
                case StudentCourseInstanceGrade.CPlus:
                    return 2.7;
                case StudentCourseInstanceGrade.C:
                    return 2.4;
                case StudentCourseInstanceGrade.DPlus:
                    return 2.2;
                case StudentCourseInstanceGrade.D:
                    return 2.0;
                case StudentCourseInstanceGrade.F:
                    return 0.0;
                case StudentCourseInstanceGrade.Not_Specified:
                    return -1;
                default:
                    return -1;
            }
        }
    }
}
