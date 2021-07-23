import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LateRegisterationService } from '../services/late-registeration.service';

@Component({
  selector: 'app-admin-late-registeration-page',
  templateUrl: './admin-late-registeration-page.component.html',
  styleUrls: ['./admin-late-registeration-page.component.css']
})
export class AdminLateRegisterationPageComponent implements OnInit {

  constructor(private lateRegisterationService : LateRegisterationService) { }

  searchForRequestByCourseCodeForm = new FormGroup({
    Course_code : new FormControl('', Validators.required)
  })

  getLateRegisterationRequestsForm = new FormGroup({
    courseInstance : new FormGroup ({
      request_id : new FormControl,
      student_id : new FormControl,
      course_instance_id : new FormControl,
      request_date : new FormControl,
      request_status : new FormControl ('', Validators.required)
    })
  })

  ngOnInit(): void {
    /*this.lateRegisterationService.getAllLateCourseRegistrationRequests().subscribe(
      requests =>{
        console.log(requests);
      },
      error =>{
        console.log(error.error);
      }
    )*/
  }

  getRequests(){
    console.log(this.searchForRequestByCourseCodeForm.value);
    this.lateRegisterationService.getCourseLateCourseRegistrationRequests(this.searchForRequestByCourseCodeForm.controls.Course_code.value).subscribe(
      requests =>{
        console.log(requests);
      },
      error =>{
        console.log(error.error);
      }
    )
  }

}
