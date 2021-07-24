import { Component, OnInit } from '@angular/core';
import { LateRegisterationService } from '../../late-registeration-module/services/late-registeration.service';
import { ViewRegisteredCoursesService } from '../services/view-registered-courses.service';

@Component({
  selector: 'app-student-course-page',
  templateUrl: './student-course-page.component.html',
  styleUrls: ['./student-course-page.component.css']
})
export class StudentCoursePageComponent implements OnInit {

  courses_list : Array<any> = []
  course_instance_list : Array<any> = []

  isNormalRegisteration : boolean = false
  isLateRegisteration : boolean = false
  isDropRegisteration : boolean = false

  constructor(private lateRegisterationService : LateRegisterationService,
    private viewRegisteredCoursesService : ViewRegisteredCoursesService) { }

    student_id = +localStorage.getItem('ID')

  ngOnInit(): void {
    this.isNormalRegisterationOpen();
    this.isLateRegisterationOpen();
    //this.isDropCourseRegistrationOpen();

    this.getRegisteredCourses();
    //this.getAllRegisteredCourseInstances();
    //this.getRegisteredCourseInstance();

  }

  getRegisteredCourses(){
    this.viewRegisteredCoursesService.getStudentRegisteredCourses(this.student_id).subscribe(
      data =>{
        this.courses_list = data;
        console.log(this.courses_list);
        var temp = this.courses_list.find(x => x.course_code === 'GE101');
        //console.log(temp);
        this.courses_list.forEach((element, index) =>{
          this.getRegisteredCourseInstance(element.course_code);
        })
      },
      error =>{
        console.log(error.error);
      }
    )
  }

  /*getAllRegisteredCourseInstances(){
    this.viewRegisteredCoursesService.getStudentRegisteredCourseInstances(this.student_id).subscribe(
      data =>{
        this.course_instance_list = data;
        console.log(this.course_instance_list);
        data.forEach((element, index) =>{
          this.filterListByCode(data, index)
        })
      },
      error =>{
        console.log(error.error);
      }
    )
  }*/

  getRegisteredCourseInstance(courseCode : string){
    this.viewRegisteredCoursesService.GetCourseStudentRegisteredCourseInstances(this.student_id, courseCode).subscribe(
      data =>{
        this.course_instance_list = data;
        console.log(this.course_instance_list);
      },
      error =>{
        console.log(error.error);
      }
    )
  }


  filterListByCode(list, index){
    var temp = list.filter(x => x.course_code === this.courses_list[index].course_code);
    console.log(temp); 
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
