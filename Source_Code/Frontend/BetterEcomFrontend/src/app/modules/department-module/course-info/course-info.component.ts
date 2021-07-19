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

  ngOnInit(): void {
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

}
