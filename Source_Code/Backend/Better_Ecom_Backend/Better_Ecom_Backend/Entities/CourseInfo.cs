using Better_Ecom_Backend.Models;
using System;
using System.Collections.Generic;

namespace Better_Ecom_Backend.Entities
{
    public class CourseInfo
    {
        public Course CourseInstance { get; set; }
        public List<String> Prerequisites { get; set; }
        public List<String> DepartmentApplicabilities { get; set; }

        public CourseInfo(Course courseInstance, List<String> prerequisites, List<String> departmentApplicabilities)
        {
            CourseInstance = courseInstance;
            Prerequisites = prerequisites;
            DepartmentApplicabilities = departmentApplicabilities;
        }

        public CourseInfo() : this(null, null, null) { }

        public void Print()
        {
            Console.WriteLine("Course Info Object Info.");
            Console.WriteLine("Course Instance Info.");
            CourseInstance.Print();

            Console.WriteLine("Prerequisites List.");
            Console.Write("{");
            foreach (String prerequisite in Prerequisites)
            {
                Console.WriteLine(prerequisite + ",");
            }
            Console.WriteLine("}.");

            Console.WriteLine("Department Applicability List.");
            Console.Write("{");
            foreach (String departmentApplicability in DepartmentApplicabilities)
            {
                Console.WriteLine(departmentApplicability + ",");
            }
            Console.WriteLine("}.");
        }
    }
}
