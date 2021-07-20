import { Component, OnInit } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
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

    courseForm = new FormGroup({
      courseArray : new FormArray([]),
    })

    get courseArray(){
      return this.courseForm.get('courseArray') as FormArray;
    }

    addcourse(){
      this.courseArray.push(this.updateCourseForm);
    }

    removeCourse(courseIndex : number){
      this.courseArray.removeAt(courseIndex);
    }

    updateCourseForm = new FormGroup({
      department_code : new FormControl('', Validators.required),
      course_code : new FormControl({disabled : true}, Validators.required),
      course_name : new FormControl('', Validators.required),
      academic_year : new FormControl('', Validators.required),
      course_description : new FormControl('', Validators.required),
      is_read_only : new FormControl({disabled : true}, Validators.required),
      is_archived : new FormControl({disabled : true}, Validators.required),
      //prerequisites : new FormControl(''),
      //departmentApplicability : new FormControl('')
    })

    courseInfoFormArray = new FormArray([])

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
    })
  }

  searchForCourse(){
    if(this.searchTypeGet.value === 'name'){
      this.departmentCoursesService.getCourseInfoByName(this.courseGet.value).subscribe(
        data =>{
          this.course_info = data;
          console.log(this.course_info);
          for(let i of data){
            console.log(i);
            this.updateCourseForm.setValue(i);
          }
          console.log(this.updateCourseForm.value);
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
          for(let i of data){
            this.updateCourseForm.setValue(i);
          }
          console.log(this.updateCourseForm.value);
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
    if(key == 'is_archived' || key == 'is_read_only' || key == 'course_code'){return false;}
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
      'Course_code' : this.course_code_get.value,
      'Academic_year' : +this.academic_year_get.value,
      'Course_name' :  this.course_name_get.value,
      'Department_code' : this.department_code_get.value,
      'Course_description' : this.course_description_get.value
    }

    console.log(this.updateCourseForm.value);
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
