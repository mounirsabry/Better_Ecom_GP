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
    }
}
