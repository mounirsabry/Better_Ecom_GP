import { Component, OnInit } from '@angular/core';
import { LateRegisterationService } from '../../late-registeration-module/services/late-registeration.service';
import { ViewRegisteredCoursesService } from '../services/view-registered-courses.service';


@Component({
  selector: 'app-instructor-course-page',
  templateUrl: './instructor-course-page.component.html',
  styleUrls: ['./instructor-course-page.component.css']
})
export class InstructorCoursePageComponent implements OnInit {

  courses_list : Array<any> = []
  course_instance_list : Array<any> = []
  combined_list : Array<any> = []

  isNormalRegisteration : boolean = false
  isLateRegisteration : boolean = false
  isDropRegisteration : boolean = false

  constructor(private lateRegisterationService : LateRegisterationService,
    private viewRegisteredCoursesService : ViewRegisteredCoursesService) { }
    
    instructor_id = +localStorage.getItem('ID')

    ngOnInit(): void {  
      this.getRegisteredCourses();
    }
  
    getRegisteredCourses(){
      this.viewRegisteredCoursesService.getInstructorRegisteredCourses(this.instructor_id).subscribe(
        data =>{
          this.courses_list = data;
          console.log(this.courses_list);
          var temp = this.courses_list.find(x => x.course_code === 'GE101');
          this.getAllRegisteredCourseInstances();
        },
        error =>{
          console.log(error.error);
        }
      )
    }
  
    getAllRegisteredCourseInstances(){
      this.viewRegisteredCoursesService.getInstructorRegisteredCourseInstances(this.instructor_id).subscribe(
        data =>{
          this.course_instance_list = data;
          console.log(this.course_instance_list);
          this.courses_list.forEach((element, index) =>{
            this.combineLists(index);
          })
        },
        error =>{
          console.log(error.error);
        }
      )
    }
  
    /*getRegisteredCourseInstance(courseCode : string){
      this.viewRegisteredCoursesService.getCourseInstructorRegisteredCourseInstances(this.student_id, courseCode).subscribe(
        data =>{
          this.course_instance_list.push(data);
        },
        error =>{
          console.log(error.error);
        }
      )
      console.log(this.course_instance_list);
    }*/
  
    combineLists(index){
      var temp = this.course_instance_list.filter(x => x.course_code === this.courses_list[index].course_code);
      var obj = {}
      for(let i of temp){
        obj = {
          'Instance_ID' : i.instance_id,
          'Course Code' : i.course_code,
          'Course Name' : this.courses_list[index].course_name,
          'Course Year' : i.course_year
        }
        this.combined_list.push(obj);
      }
      console.log(this.combined_list);
    }
  
    checkInstanceID(key){
      if(key === 'Instance_ID'){
        return true;
      }
      else {return false;}
    }
  
    filterListByCode(list, index){
      var temp = list.filter(x => x.course_code === this.courses_list[index].course_code);
      return temp;
    }
  
    isNormalRegisterationOpen(){
      this.lateRegisterationService.getIsNormalCourseRegistrationOpen().subscribe(
        response =>{
          this.isNormalRegisteration = response;
          console.log(this.isNormalRegisteration);
        },
        error =>{
          console.log(error.error);
        }
      )
    }
  
    isLateRegisterationOpen(){
      this.lateRegisterationService.getIsLateCourseRegistrationOpen().subscribe(
        response =>{
          this.isLateRegisteration = response;
          console.log(this.isLateRegisteration);
        },
        error =>{
          console.log(error.error);
        }
      )
    }
  
    /*isDropCourseRegistrationOpen(){
      this.lateRegisterationService.getIsDropCourseRegistrationOpen().subscribe(
        response =>{
          this.isDropRegisteration = response;
          console.log(this.isDropRegisteration);
        },
        error =>{
          console.log(error.error);
        }
      )
    }*/
  }
  
