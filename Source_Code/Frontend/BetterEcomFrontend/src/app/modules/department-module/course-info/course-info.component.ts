import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { DepartmentCoursesService } from '../services/department-courses.service';

@Component({
  selector: 'app-course-info',
  templateUrl: './course-info.component.html',
  styleUrls: ['./course-info.component.css']
})
export class CourseInfoComponent implements OnInit {

  course_info : Array<any> = []
  updateFlag : boolean = false;
  type : string

  constructor(private activatedRoute:ActivatedRoute,
    private departmentCoursesService:DepartmentCoursesService) { }

    courseInfoForm = new FormGroup({
      course : new FormControl('', [Validators.required]),
      searchType : new FormControl('', [Validators.required])
    })

    get courseGet(){
      return this.courseInfoForm.get('course');
    }

    get searchTypeGet(){
      return this.courseInfoForm.get('searchType');
    }

    updateCourseForm = new FormGroup({
      UserID : new FormControl('', Validators.required),
      department_code : new FormControl('', Validators.required),
      course_code : new FormControl('', Validators.required),
      course_name : new FormControl('', Validators.required),
      academic_year : new FormControl('', Validators.required),
      course_description : new FormControl('', Validators.required),
      is_read_only : new FormControl('', Validators.required),
      is_archived : new FormControl('', Validators.required),
      //prerequisites : new FormControl(''),
      //departmentApplicability : new FormControl('')
    })

    get department_code_get(){
      return this.updateCourseForm.get('department_code');
    }

    get course_code_get(){
      return this.updateCourseForm.get('course_code');
    }

    get course_name_get(){
      return this.updateCourseForm.get('course_name');
    }

    get academic_year_get(){
      return this.updateCourseForm.get('academic_year');
    }

    get course_description_get(){
      return this.updateCourseForm.get('course_description');
    }


    get is_read_get(){
      return this.updateCourseForm.get('is_read_only');
    }

    get is_archived_get(){
      return this.updateCourseForm.get('is_archived');
    }


  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params: ParamMap) =>{
      this.type = localStorage.getItem('type');
      this.updateCourseForm.controls['UserID'].setValue(+localStorage.getItem('ID'));
    })
    var course = {
      'UserID' : +localStorage.getItem('ID'),
      'courseCode' : this.course_code_get.value,
      'academicYear' : this.academic_year_get.value,
      'courseName' : this.course_name_get.value,
      'departmentCode' : this.department_code_get.value,
      'courseDescription' : this.course_description_get.value
    }
  }

  searchForCourse(){
    if(this.searchTypeGet.value === 'name'){
      this.departmentCoursesService.getCourseInfoByName(this.courseGet.value).subscribe(
        data =>{
          this.course_info = data;
          console.log(this.course_info);
        },
        error =>{
          console.log(error.error);
          alert("failed");
        }
      )
    }
    else{
      this.departmentCoursesService.getCourseInfoByCode(this.courseGet.value).subscribe(
        data =>{
          this.course_info = data;
          console.log(this.course_info);
        },
        error =>{
          console.log(error.error);
          alert("failed");
        }
      )
    }
  }

  objectValues(obj){
    return Object.values(obj);
  }

  objectKeys(obj){
    return Object.keys(obj);
  }

  isAdmin(){
    if(this.type == 'admin'){
      return true;
    }
    else{
      return false;
    }
  }

  inputType(key){
    if(key == "academic_year"){
      return "number"
    }else{
      return "text"
    }
  }

  checkKey(key){
    if(key == 'is_read_only'){return false;}
    else if (key == 'is_archived'){return false;}
    else{return true;}
  }

  changeColor(name){
    return (this.updateCourseForm.get(name).dirty? 'red' : 'black')
  }

  updateCourse(){

    /*if(this.is_read_get.value == 'false'){this.updateCourseForm.controls['is_read_only'].setValue(0);}
    else if(this.is_read_get.value == 'true'){this.updateCourseForm.controls['is_read_only'].setValue(1);}

    if(this.is_archived_get.value == 'false'){this.updateCourseForm.controls['is_archived'].setValue(0);}
    else if(this.is_archived_get.value == 'true'){this.updateCourseForm.controls['is_archived'].setValue(1);}*/

    console.log(this.course_info[0].course_code);

    var course = {
      'UserID' : +localStorage.getItem('ID'),
      'courseCode' : (this.course_code_get.value == "")? this.course_info[0].course_code : this.course_code_get.value,
      'academicYear' : this.academic_year_get.value,
      'courseName' : this.course_name_get.value,
      'departmentCode' : this.department_code_get.value,
      'courseDescription' : this.course_description_get.value
    }

    console.log(course);

    this.departmentCoursesService.updateCourseInformation(course).subscribe(
      data =>{
        alert("Course updated");
      },
      error =>{
        console.log(error.error);
        alert("failed");
      }
    )
  }

}
