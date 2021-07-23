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
  searchFlag : boolean = false //to show the update button after the search not before

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
      courseInstanceForm : new FormGroup({
        department_code : new FormControl('', Validators.required),
        course_code : new FormControl({disabled : true}, Validators.required),
        course_name : new FormControl('', Validators.required),
        academic_year : new FormControl('', Validators.required),
        course_description : new FormControl('', Validators.required),
        is_read_only : new FormControl({disabled : true}, Validators.required),
        is_archived : new FormControl({disabled : true}, Validators.required),
      }),
      updateCoursePrerequisiteForm : new FormGroup({
        prerequisites : new FormControl('')
      }),
      updateCourseDepartmentApplicabilityForm : new FormGroup({
        departmentApplicability : new FormControl('')
      })
    })

    courseInfoFormArray = new FormArray([])

    get department_code_get(){
      return this.updateCourseForm.controls['courseInstanceForm'].value.department_code;
    }

    get course_code_get(){
      return this.updateCourseForm.controls['courseInstanceForm'].value.course_code;
    }

    get course_name_get(){
      return this.updateCourseForm.controls['courseInstanceForm'].value.course_name;
    }

    get academic_year_get(){
      return this.updateCourseForm.controls['courseInstanceForm'].value.academic_year;
    }

    get course_description_get(){
      return this.updateCourseForm.controls['courseInstanceForm'].value.course_description;
    }


    get is_read_get(){
      return this.updateCourseForm.controls['courseInstanceForm'].value.is_read_only;
    }

    get is_archived_get(){
      return this.updateCourseForm.controls['courseInstanceForm'].value.is_archived;
    }



    get prerequisites_get(){
      return this.updateCourseForm.controls['updateCoursePrerequisiteForm'].value.prerequisites;
    }

    get departmentApplicability_get(){
      return this.updateCourseForm.controls['updateCourseDepartmentApplicabilityForm'].value.departmentApplicability;
    }

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params: ParamMap) =>{
      this.type = localStorage.getItem('type');
    })
  }

  searchForCourse(){
    this.course_info = [];
    this.prerequisites_list = [];
    this.departmentApplicability_list = [];
    if(this.searchTypeGet.value === 'name'){
      this.departmentCoursesService.getCourseInfoByName(this.courseGet.value).subscribe(
        data =>{
          console.log(data);
          data.forEach((element,index) => {
            this.course_info.push(element.courseInstance)
          });
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
          console.log(data);
          this.course_info.push(data.courseInstance);
          for(let m of data.prerequisites){
            this.prerequisites_list.push(m);
          }
          console.log(data.prerequisites);

          for(let n of data.departmentApplicabilities){
            this.departmentApplicability_list.push(n);
          }

          console.log(this.course_info);
          console.log(this.departmentApplicability_list);
          console.log(this.prerequisites_list);

          this.searchFlag = true;
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

  checkSearchFlag(){
    return this.searchFlag
  }

  updateCourse(){
    
    var ay, cn, dc, cd;

    if(this.academic_year_get.length < 1) {
      console.log('hey');
      ay = this.course_info[0].academic_year;}
    else {ay = +this.academic_year_get;}

    if(this.course_name_get < 1) {cn = this.course_info[0].course_name;}
    else {cn = this.course_name_get;}
    
    if(this.department_code_get < 1) {dc = this.course_info[0].department_code;}
    else {dc = this.department_code_get;}

    if(this.course_description_get < 1) {cd = this.course_info[0].course_description;}
    else {cd = this.course_description_get;}

    var course = {
      'UserID' : +localStorage.getItem('ID'),
      'Course_code' : this.course_info[0].course_code,
      'Academic_year' : ay,
      'Course_name' :  cn ,
      'Department_code' : dc,
      'Course_description' : cd
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

  addPrerequisite(){
    this.prerequisites_list.push(this.prerequisites_get);
    this.updateCourseForm.controls.updateCoursePrerequisiteForm.reset();
  }

  removePrerequisites(prerequisite : any){
    console.log(prerequisite);
    this.prerequisites_list.forEach((element, index)=>{
      if(element == prerequisite) this.prerequisites_list.splice(index, 1);
    })
  }

  addDepartmentApplicability(){
    this.departmentApplicability_list.push(this.departmentApplicability_get);
    this.updateCourseForm.controls['updateCourseDepartmentApplicabilityForm'].reset();
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
      'Course_code' : this.course_info[0].course_code,
      'prerequisites' : this.prerequisites_list
    }

    console.log(course_prerequisite);

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
      'Course_code' : this.course_info[0].course_code,
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
    
    var course = this.courseGet.value;
    console.log(course);
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
