import { Component, OnInit } from '@angular/core';
import { LateRegisterationService } from '../../late-registeration-module/services/late-registeration.service';

@Component({
  selector: 'app-student-course-page',
  templateUrl: './student-course-page.component.html',
  styleUrls: ['./student-course-page.component.css']
})
export class StudentCoursePageComponent implements OnInit {

  isNormalRegisteration : boolean = false;
  isLateRegisteration : boolean = false;
  isDropRegisteration : boolean = false;

  constructor(private lateRegisterationService : LateRegisterationService) { }

  ngOnInit(): void {
    this.isNormalRegisterationOpen();
    this.isLateRegisterationOpen();
    this.isDropCourseRegistrationOpen();
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

  isDropCourseRegistrationOpen(){
    this.lateRegisterationService.getIsDropCourseRegistrationOpen().subscribe(
      response =>{
        this.isDropRegisteration = response;
        console.log(this.isDropRegisteration);
      },
      error =>{
        console.log(error.error);
      }
    )
  }
}
