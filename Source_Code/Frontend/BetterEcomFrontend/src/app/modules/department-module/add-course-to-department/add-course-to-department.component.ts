import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { DepartmentCoursesService } from '../services/department-courses.service';

@Component({
  selector: 'app-add-course-to-department',
  templateUrl: './add-course-to-department.component.html',
  styleUrls: ['./add-course-to-department.component.css']
})
export class AddCourseToDepartmentComponent implements OnInit {

  userID : number
  data : Array<any> = []
  prerequisites_list : Array<string> = []
  departmentApplicability_list : Array<string> = []

  constructor(private activatedRoute:ActivatedRoute,
    private departmentCoursesService:DepartmentCoursesService) { }

    addCourseToDepartmentForm = new FormGroup({
      UserID : new FormControl('',),
      Department_code : new FormControl('', Validators.required),
      Course_code : new FormControl('', Validators.required),
      Course_name : new FormControl('', Validators.required),
      academic_year : new FormControl('', Validators.required),
      course_description : new FormControl('', Validators.required),
      prerequisites : new FormControl(''),
      departmentApplicability : new FormControl('')
    })


    get department_code_get(){
      return this.addCourseToDepartmentForm.get('Department_code');
    }

    get course_code_get(){
      return this.addCourseToDepartmentForm.get('Course_code');
    }

    get course_name_get(){
      return this.addCourseToDepartmentForm.get('Course_name');
    }

    get academic_year_get(){
      return this.addCourseToDepartmentForm.get('academic_year');
    }

    get course_description_get(){
      return this.addCourseToDepartmentForm.get('course_description');
    }

    get prerequisites_get(){
      return this.addCourseToDepartmentForm.get('prerequisites');
    }

    get departmentApplicability_get(){
      return this.addCourseToDepartmentForm.get('departmentApplicability');
    }


  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params: ParamMap) => {
      var ID = localStorage.getItem('ID');
      this.userID = +ID;
      this.addCourseToDepartmentForm.controls['UserID'].setValue(this.userID);
    })
  }

  addPrerequisite(){
    this.prerequisites_list.push(this.prerequisites_get.value);
    this.addCourseToDepartmentForm.controls['prerequisites'].reset();
  }

  removePrerequisites(prerequisite : any){
    console.log(prerequisite);
    this.prerequisites_list.forEach((element, index)=>{
      if(element == prerequisite) this.prerequisites_list.splice(index, 1);
    })
  }

  addDepartmentApplicability(){
    this.departmentApplicability_list.push(this.departmentApplicability_get.value);
    this.addCourseToDepartmentForm.controls['departmentApplicability'].reset();
  }

  removeDepartmentApplicability(depApp : any){
    console.log(depApp);
    this.departmentApplicability_list.forEach((element, index)=>{
      if(element == depApp) this.departmentApplicability_list.splice(index, 1);
    })
  }

  isDepAppEmpty(){
    if(this.departmentApplicability_list.length < 1){
      return true;
    }
    else{
      return false;
    }
  }

  addCourseToDepartment(){
    var temp = {
      'UserID' : this.userID,
      'Department_code' : this.department_code_get.value,
      'Course_code' : this.course_code_get.value,
      'Course_name' : this.course_name_get.value,
      'Academic_year' : +this.academic_year_get.value,
      'Course_description' : this.course_description_get.value,
      'Prerequisites' : this.prerequisites_list,
      'DepartmentApplicability' : this.departmentApplicability_list
    }
    console.log(temp);
    this.departmentCoursesService.addCourse(temp).subscribe(
      data =>{
        alert("Course added");
      },
      error =>{
        console.log(error.error);
        alert("failed");
      }
    )
  }

}
