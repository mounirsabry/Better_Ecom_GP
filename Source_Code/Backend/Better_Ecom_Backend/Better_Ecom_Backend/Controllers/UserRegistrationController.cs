using Better_Ecom_Backend.Helpers;
using Better_Ecom_Backend.Models;
using DataLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        private IConfiguration _config;
        private IDataAccess _data;

        public UserRegistrationController(IConfiguration config, IDataAccess data)
        {
            _config = config;
            _data = data;
        }

        [HttpPost("AddNewStudent")]
        public IActionResult AddNewStudent([FromBody] dynamic studentData)
        {
            JsonElement studentJson = (JsonElement)studentData;
            Student newStudent = new(studentJson);
            newStudent.Print();
            if (CheckSystemUserData(newStudent) == false)
            {
                return BadRequest(new { Message = "data common between users part is not valid." });
            }
            if (CheckStudentData(newStudent) == false)
            {
                return BadRequest(new { Messsgae = "student data part is not valid." });
            }

            //Checks if a student (system_user) with the same national ID and nationality
            //exists in the database.
            //The combination of the nationality and national ID can only occurs twice at most 
            //at the database.
            //One of them represents a student and the other represent an instructor.
            //Representing the case that one person can be an instructor and a student (after-graduation studies) at the same time.
            string checkUserExistenceSQL = @"SELECT system_user_id, nationality, national_id
                                        FROM system_user
                                        WHERE national_id = @NationalID
                                        AND nationality = @Nationality;";
            var parameters = new
            {
                NationalID = newStudent.National_id,
                Nationality = newStudent.Nationality
            };
            List<System_user> users;
            users = _data.LoadData<System_user, dynamic>(checkUserExistenceSQL, parameters, _config.GetConnectionString(Constants.CurrentDBConnectionStringName));
            //One person can not be student, instructor, admin at the same time.
            if (users.Count >= 2)
            {
                return BadRequest(new { Message = "two users or more with the same natioanl id and nationality already exists." });
            }
            else if (users.Count == 1)
            {
                //National ID and nationality combination alraedy exists in the database.
                //If the user is an instructor, then the registration will continue normally.
                //If the user is a student, then the process will halt 
                //since you can not register two student with the same national ID and nationality combination.
                int systemUserID = users[0].System_user_id;
                int firstDigit = GetFirstDigit(systemUserID);

                //Another student registered with the same national ID and nationality.
                if (firstDigit == 2)
                {
                    return BadRequest(new { Message = "a student with the same national id and nationality already exists." });
                }
            }

            //Constructing the ID.
            int currentYear = GetCurrentYear();
            //Gets the last inserted ID in the student table.
            string getLastIDSQL = @"SELECT MAX(student_id) FROM student;";
            List<int> IDs = _data.LoadData<int, dynamic>(getLastIDSQL, new { }, _config.GetConnectionString(Constants.CurrentDBConnectionStringName));

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

            string systemUserInsertionSQL = @"INSERT INTO system_user
                                        VALUES (@ID, NULL, @FullName, @Email, @Address, @PhoneNumber, @MobileNumber,
                                        @Nationality, @NationalID, @BirthDate, @Gender, @AdditionalInfo);";
            var parameters2 = new
            {
                ID = newID,
                FullName = newStudent.Full_name,
                Email = newStudent.Email,
                Address = newStudent.Address,
                PhoneNumber = newStudent.Phone_number,
                MobileNumber = newStudent.Mobile_number,
                Nationality = newStudent.Nationality,
                NationalID = newStudent.National_id,
                BirthDate = newStudent.Birth_date,
                Gender = newStudent.Gender,
                AdditionalInfo = newStudent.Additional_info
            };
            int systemUserInsertionState = _data.SaveData<dynamic>(systemUserInsertionSQL, parameters2, _config.GetConnectionString(Constants.CurrentDBConnectionStringName));
            if (systemUserInsertionState == -1)
            {
                return BadRequest(new { Message = "unknown error." });
            }

            string studentInsertionSQL = @"INSERT INTO student
                                VALUES(@ID, @HighSchoolType, @EntranceYear, @GPA, @Department, @AcademicYear);";
            var parameters3 = new
            {
                ID = newID,
                HighSchoolType = newStudent.High_school_type,
                EntranceYear = newStudent.Entrance_year,
                GPA = newStudent.GPA,
                Department = newStudent.Department,
                AcademicYear = newStudent.Academic_year
            };
            int studentInsertionState = _data.SaveData<dynamic>(studentInsertionSQL, parameters3, _config.GetConnectionString(Constants.CurrentDBConnectionStringName));
            if (studentInsertionState == -1)
            {
                return BadRequest(new { Message = "unknown error." });
            }

            //Adds the new student to the database.
            //The student will have the next available ID in the students table.
            //The four first digits of the ID should be the current Year.
            //The process of adding the student to the students table and the system_users table
            //should be a transaction, which means it should be done all or nothing.

            //Should return the new added student object.
            return Ok("Not Implemented Yet!");
        }

        [HttpPost("AddNewInstructor")]
        public IActionResult AddNewInstructor([FromBody] dynamic instructorData)
        {
            JsonElement instructorJson = (JsonElement)instructorData;
            Instructor newInstructor = new(instructorJson);
            newInstructor.Print();

            //Adds the new instructor to the database.
            //The new ID should start with 3 followed by the next available number in
            //the instructors table.
            //Should be a transaction.

            //Should return the new added instructor object.
            return Ok("Not Implemented Yet!");
        }

        private bool CheckSystemUserData(System_user user)
        {
            if (user.System_user_id != -1)
            {
                return false; //The user should not specify an ID to be used.
            }
            if (user.User_password != null || user.User_password != "")
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
            if (user.Gender != "Male" || user.Gender != "Female")
            {
                return false;
            }

            return true;
        }

        private bool CheckStudentData(Student student)
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
            if (student.Department == null || student.Department == "")
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

        private bool CheckInstructorData(Instructor instructor)
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

        private int GetFirstDigit(int number)
        {
            return (int)number.ToString()[0] - 48;
        }

        private int GetCurrentYear()
        {
            string currentYearString = DateTime.Today.ToString("yyyy");
            return Int32.Parse(currentYearString);
        }
    }
}
