using Better_Ecom_Backend.Helpers;
using Better_Ecom_Backend.Models;
using DataLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Better_Ecom_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _data;

        private static readonly Object AddStudentLock = new();
        private static readonly Object AddInstructorLock = new();

        public UserRegistrationController(IConfiguration config, IDataAccess data)
        {
            _config = config;
            _data = data;
        }

        /// <summary>
        /// Register new student to database.
        /// </summary>
        /// <param name="studentData">json object containing new student data.</param>
        /// <returns>Created and new student object if successful BadRequest otherwise.</returns>
        [Authorize(Roles = "admin")]
        [HttpPost("AddNewStudent")]
        public IActionResult AddNewStudent([FromBody] JsonElement studentJson)
        {
            Student newStudent = new(studentJson);
            if (CheckSystemUserData(newStudent) == false)
            {
                return BadRequest(new { Message = "data common between users part is not valid." });
            }
            else if (CheckStudentData(newStudent) == false)
            {
                return BadRequest(new { Messsgae = "student data part is not valid." });
            }
            else if (HelperFunctions.IsDepartmentCodeValid(_config, _data, newStudent.Department_code) == false)
            {
                return BadRequest(new { Message = "department code is not valid." });
            }
            /*
            Checks if a student(system_user) with the same national ID and nationality
            exists in the database.
            The combination of the nationality and national ID can only occurs twice at most
            at the database.
            One of them represents a student and the other represent an instructor.
            Representing the case that one person can be an instructor and a student
            (after-graduation studies for example) at the same time.
            */
            string checkUserExistenceSQL = "SELECT system_user_id, nationality, national_id" + "\n"
                                        + "FROM system_user" + "\n"
                                        + "WHERE national_id = @NationalID" + "\n"
                                        + "AND nationality = @Nationality;";
            var parameters = new
            {
                NationalID = newStudent.National_id,
                newStudent.Nationality
            };
            var users = _data.LoadData<dynamic, dynamic>(checkUserExistenceSQL, parameters, _config.GetConnectionString("Default"));

            if (users is null)
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
            //One person can not be student, instructor, admin at the same time.
            if (users.Count >= 2)
            {
                return BadRequest(new { Message = "two users or more with the same natioanl id and nationality combination already exists." });
            }
            else if (users.Count == 1)
            {
                /*
                National ID and nationality combination alraedy exists in the database.
                If the user is an instructor, then the registration will continue normally.
                If the user is a student, then the process will halt
                since you can not register two students with the same national ID and nationality combination.
                */
                int systemUserID = users[0].system_user_id;
                int firstDigit = HelperFunctions.GetFirstDigit(systemUserID);

                //The registered data is for admin_user, the user can not be an admin and student/instructor at the same time.
                if (firstDigit == 1)
                {
                    return BadRequest(new { Message = "the entered nationality and national id combination is reserved." });
                }
                //Another student registered with the same national ID and nationality.
                if (firstDigit == 2)
                {
                    return BadRequest(new { Message = "a student with the same national id and nationality combination already exists." });
                }
            }

            List<int> states;

            lock (AddStudentLock)
            {
                int newID = GetNextStudentID();
                newStudent.System_user_id = newID;
                newStudent.Student_id = newID;
                states = InsertNewStudent(newStudent);
            }

            if (states.Contains(-1))
            {
                return BadRequest(new { Message = "unknown error." });
            }

            return Created("/UserRegistration/AddNewStudent", newStudent);
        }

        /// <summary>
        /// Add new Instructor to database.
        /// </summary>
        /// <param name="instructorData">Json object containing instructor data</param>
        /// <returns>Created and new instructor object, BadRequest otherwise.</returns>
        [Authorize(Roles = "admin")]
        [HttpPost("AddNewInstructor")]
        public IActionResult AddNewInstructor([FromBody] JsonElement instructorJson)
        {
            Instructor newInstructor = new(instructorJson);

            if (CheckSystemUserData(newInstructor) == false)
            {
                return BadRequest(new { Message = "data common between users part is not valid." });
            }
            if (CheckInstructorData(newInstructor) == false)
            {
                return BadRequest(new { Message = "instructor data part is not valid." });
            }
            if (HelperFunctions.IsDepartmentCodeValid(_config, _data, newInstructor.Department_code) == false)
            {
                return BadRequest(new { Message = "department code is not valid." });
            }

            string checkUserExistenceSQL = "SELECT system_user_id, nationality, national_id" + "\n"
                                        + "FROM system_user" + "\n"
                                        + "WHERE national_id = @NationalID" + "\n"
                                        + "AND nationality = @Nationality;";
            var parameters = new
            {
                NationalID = newInstructor.National_id,
                newInstructor.Nationality
            };

            var users = _data.LoadData<dynamic, dynamic>(checkUserExistenceSQL, parameters, _config.GetConnectionString("Default"));
            //One person can not be student, instructor, admin at the same time.
            if (users.Count >= 2)
            {
                return BadRequest(new { Message = "two users or more with the same natioanl id and nationality combination already exists." });
            }
            else if (users.Count == 1)
            {
                /*
                National ID and nationality combination alraedy exists in the database.
                If the already registered user is a student, then the registration will continue normally.
                If the already registered user is an instructor, then the process will halt
                since you can not register two instructors with the same national ID and nationality combination.
                */
                int systemUserID = users[0].system_user_id;
                int firstDigit = HelperFunctions.GetFirstDigit(systemUserID);

                //The registered data is for admin_user, the user can not be an admin and student/instructor at the same time.
                if (firstDigit == 1)
                {
                    return BadRequest(new { Message = "the entered nationality and national id combination is reserved." });
                }
                //Another instructor registered with the same national ID and nationality.
                if (firstDigit == 3)
                {
                    return BadRequest(new { Message = "an instructor with the same national id and nationality combination already exists." });
                }
            }

            List<int> states;

            lock (AddInstructorLock)
            {
                int newID = GetNextInstructorID();
                newInstructor.System_user_id = newID;
                newInstructor.Instructor_id = newID;
                states = InsertNewInstructor(newInstructor);
            }

            if (states.Contains(-1))
            {
                return BadRequest(new { Message = "unknown error." });
            }

            return Created("/UserRegistration/AddNewInstructor", newInstructor);
        }

        private static bool CheckSystemUserData(System_user user)
        {
            if (user.System_user_id != -1)
            {
                return false; //The user should not specify an ID to be used.
            }
            if (user.User_password != null || user.User_password == "")
            {
                return false; //The user should not specify a password at this phase.
            }
            if (user.Full_name == null || user.Full_name == "")
            {
                return false; //Full Name can not be empty.
            }
            if (user.Address == null || user.Address == "")
            {
                return false; //Address should not be empty.
            }
            if (user.Nationality == null || user.Nationality == "")
            {
                return false;
            }
            if (user.National_id == null || user.National_id == "")
            {
                return false;
            }

            //User did not enter a date (null) or he entered empty string or some invalid date.
            //Then The C# assigned the min value to the date when reading the data.
            if (user.Birth_date == DateTime.MinValue)
            {
                return false;
            }
            //Checks if the date is less the 5 years old, if so, refuse.
            //I think that 5 years old as minimum age requirement is a reasonable assumption.
            DateTime today5YearsAgo = DateTime.Today.AddYears(-5);
            if (user.Birth_date > today5YearsAgo)
            {
                return false;
            }
            //Checks if the date is older than 200 years old, if so refuse.
            // 200 years old is a very big number and very probably not intended.
            //So I assume that the user has entered something wrong and he should be ware to correct it.
            //However, if the scenairo that a real 200+ years old user wants to register in the system (highly unlikely)
            //then he will not be able to pass this, thus, his/her registration will require a manual intervention.
            DateTime today200YearsAgo = DateTime.Today.AddYears(-200);
            if (user.Birth_date < today200YearsAgo)
            {
                return false;
            }

            //Only Male and Female are accepted, not specified or any other value is not allowed.
            //For the sake of uniqification, we foced it to be case-sensitive.
            if (user.Gender != "Male" && user.Gender != "Female")
            {
                return false;
            }

            return true;
        }

        private static bool CheckStudentData(Student student)
        {
            //The user/Frontend should not specify any value for the ID that we be added to the system.
            //The ID should be fully automatically constructed by the system.
            if (student.Student_id != -1)
            {
                return false;
            }
            if (student.High_school_type == null || student.High_school_type == "")
            {
                return false;
            }
            if ((student.Entrance_year < 0 && student.Entrance_year != -1))
            {
                return false;
            }
            //Here I refuse that the user enters any value for the GPA.
            //I assume that the GPA should be calculated automatically without the user intervention.
            if (student.GPA != 0.0)
            {
                return false;
            }
            //Should be modified to reflect the availabe departments in the database.
            if (student.Department_code == null || student.Department_code == "")
            {
                return false;
            }
            // -1 Reflect not-specified academic year.
            if ((student.Academic_year <= 0 && student.Academic_year != -1))
            {
                return false;
            }

            return true;
        }

        private static bool CheckInstructorData(Instructor instructor)
        {
            //The same idea as student_id.
            if (instructor.Instructor_id != -1)
            {
                return false;
            }
            if (instructor.University == null || instructor.University == "")
            {
                return false;
            }
            if ((instructor.Graduation_year <= 0 && instructor.Graduation_year != -1))
            {
                return false;
            }

            return true;
        }

        private int GetNextStudentID()
        {
            //Constructing the ID.
            int currentYear = TimeUtilities.GetCurrentYear();
            //Gets the last inserted ID in the student table.
            string getLastIDSQL = @"SELECT MAX(student_id) FROM student;";
            List<int> IDs = _data.LoadData<int, dynamic>(getLastIDSQL, new { }, _config.GetConnectionString("Default"));

            int newID;
            if (IDs.Count == 0)
            {
                //First Student in the database, very special and simple case.
                string newIDString = currentYear + "0001";
                newID = Int32.Parse(newIDString);
            }
            else
            {
                int lastInsertedID = IDs[0];
                string lastInsertedIDYearString = lastInsertedID.ToString().Substring(0, 4);
                int lastInsertedIDYear = Int32.Parse(lastInsertedIDYearString);

                if (lastInsertedIDYear == currentYear)
                {
                    newID = lastInsertedID + 1;
                }
                else
                {
                    //First student to be inserted this year, Congrats :)
                    string newIDString = currentYear + "0001";
                    newID = Int32.Parse(newIDString);
                }
            }

            return newID;
        }

        private int GetNextInstructorID()
        {
            string getLastIDSQL = "SELECT MAX(instructor_id) FROM instructor;";
            List<int> IDs = _data.LoadData<int, dynamic>(getLastIDSQL, new { }, _config.GetConnectionString("Default"));

            int newID;
            if (IDs.Count == 0)
            {
                //First instructor in the database, very special and simple case.
                newID = 31;
            }
            else
            {
                int lastIDInserted = IDs[0];
                newID = lastIDInserted + 1;
            }
            return newID;
        }

        private List<int> InsertNewStudent(Student newStudent)
        {
            string systemUserInsertionSQL = "INSERT INTO system_user" + "\n"
                                       + "VALUES (@System_user_id, NULL, @Full_name, @Email, @Address, @Phone_number, @Mobile_number," + "\n"
                                       + "@Nationality, @National_id, @Birth_date, @Gender, @Additional_info);";

            string studentInsertionSQL = "INSERT INTO student" + "\n"
                                       + "VALUES(@Student_id, @Department_code, @High_school_type, @Entrance_year, DEFAULT, @Academic_year);";

            List<string> insertionQueries = new()
            {
                systemUserInsertionSQL,
                studentInsertionSQL
            };

            List<dynamic> parametersList = new()
            {
                newStudent,
                newStudent
            };

            List<int> states = _data.SaveDataTransaction<dynamic>(insertionQueries, parametersList, _config.GetConnectionString("Default"));
            return states;
        }

        private List<int> InsertNewInstructor(Instructor newInstructor)
        {
            string systemUserInsertionSQL = "INSERT INTO system_user" + "\n"
                                       + "VALUES (@System_user_id, NULL, @Full_name, @Email, @Address, @Phone_number, @Mobile_number," + "\n"
                                       + "@Nationality, @National_id, @Birth_date, @Gender, @Additional_info);";

            string instructorInsertionSQL = "INSERT INTO instructor" + "\n"
                                       + "VALUES (@Instructor_id, @Department_code, @University, @Graduation_year, @Contact_info);";

            List<string> insertionQueries = new()
            {
                systemUserInsertionSQL,
                instructorInsertionSQL
            };

            List<dynamic> parametersList = new()
            {
                newInstructor,
                newInstructor
            };

            List<int> states = _data.SaveDataTransaction<dynamic>(insertionQueries, parametersList, _config.GetConnectionString("Default"));
            return states;
        }
    }
}
