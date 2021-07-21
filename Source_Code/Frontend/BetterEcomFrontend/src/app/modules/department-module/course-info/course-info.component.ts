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
  prerequisites_list : Array<string> = []
  departmentApplicability_list : Array<string> = []

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
      department_code : new FormControl('', Validators.required),
      course_code : new FormControl({disabled : true}, Validators.required),
      course_name : new FormControl('', Validators.required),
      academic_year : new FormControl('', Validators.required),
      course_description : new FormControl('', Validators.required),
      is_read_only : new FormControl({disabled : true}, Validators.required),
      is_archived : new FormControl({disabled : true}, Validators.required),
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



    updateCoursePrerequisiteForm = new FormGroup({
      prerequisites : new FormControl('')
    })

    get prerequisites_get(){
      return this.updateCoursePrerequisiteForm.get('prerequisites');
    }

    updateCourseDepartmentApplicabilityForm = new FormGroup({
      departmentApplicability : new FormControl('')
    })

    get departmentApplicability_get(){
      return this.updateCourseDepartmentApplicabilityForm.get('departmentApplicability');
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

  addPrerequisite(){
    this.prerequisites_list.push(this.prerequisites_get.value);
    this.updateCoursePrerequisiteForm.controls['prerequisites'].reset();
  }

  removePrerequisites(prerequisite : any){
    console.log(prerequisite);
    this.prerequisites_list.forEach((element, index)=>{
      if(element == prerequisite) this.prerequisites_list.splice(index, 1);
    })
  }

  addDepartmentApplicability(){
    this.departmentApplicability_list.push(this.departmentApplicability_get.value);
    this.updateCourseDepartmentApplicabilityForm.controls['departmentApplicability'].reset();
  }

  removeDepartmentApplicability(depApp : any){
    console.log(depApp);
    this.departmentApplicability_list.forEach((element, index)=>{
      if(element == depApp) this.departmentApplicability_list.splice(index, 1);
    })
  }

  isListEmpty(list){
    if(list.length < 1){
      return true;
    }
    else{
      return false;
    }
  }

  updateCoursePrerequisite(){
    var course_prerequisite = {
      'UserID' : +localStorage.getItem('ID'),
      'Course_code' : this.course_code_get.value,
      'prerequisites' : this.prerequisites_list
    }

    this.departmentCoursesService.updateCoursePrerequisiteInfo(course_prerequisite).subscribe(
      data =>{
        alert("Course Prerequisite updated");
      },
      error =>{
        console.log(error.error);
        alert("failed");
      }
    )
  }

  updateCourseDepartmentApplicability(){
    var course_dep_applicablitiy = {
      'UserID' : +localStorage.getItem('ID'),
      'Course_code' : this.course_code_get.value,
      'departmentApplicability' : this.departmentApplicability_list
    }

    this.departmentCoursesService.updateCourseDepartmentApplicabilityInfo(course_dep_applicablitiy).subscribe(
      data =>{
        alert("Course Department Applicalbility updated");
      },
      error =>{
        console.log(error.error);
        alert("failed");
      }
    )
  }

  archiveCourse(){
    var course = {
      'UserID' : +localStorage.getItem('ID'),
      'Course_code' : this.course_code_get.value,
      'is_archived' : this.is_archived_get.value
    }

    this.departmentCoursesService.archiveCourse(course).subscribe(
      data =>{
        alert("Course archived");
      },
      error =>{
        console.log(error.error);
        alert("failed");
      }
    )
  }

}
