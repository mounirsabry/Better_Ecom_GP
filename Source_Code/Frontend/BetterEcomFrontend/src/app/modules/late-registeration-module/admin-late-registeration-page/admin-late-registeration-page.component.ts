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

  get course_code_get(){
    return this.searchForRequestByCourseCodeForm.get('Course_code');
  }

  searchForRequestByStudentIDForm = new FormGroup({
    Student_ID : new FormControl('', Validators.required)
  })

  get student_id_get(){
    return this.searchForRequestByStudentIDForm.get('Student_ID');
  }

  getLateRegisterationRequestsForm = new FormGroup({
    courseInstance : new FormGroup ({
      request_id : new FormControl,
      student_id : new FormControl,
      course_instance_id : new FormControl,
      request_date : new FormControl,
      request_status : new FormControl ('', Validators.required)
    })
  })

  late_requests_list : Array<any> = []

  ngOnInit(): void {
  }

  getAllRequests(){
    this.lateRegisterationService.getAllLateCourseRegistrationRequests().subscribe(
      requests =>{
        console.log(requests);
        this.late_requests_list = requests;
        console.log(this.late_requests_list);
      },
      error =>{
        console.log(error.error);
      }
    )
  }

  getCourseRequests(){
    console.log(this.course_code_get.value);
    this.lateRegisterationService.getCourseLateCourseRegistrationRequests(this.course_code_get.value).subscribe(
      requests =>{
        console.log(requests);
      },
      error =>{
        console.log(error.error);
      }
    )
  }

  getStudentRequests(){
    console.log(this.student_id_get.value);
    this.lateRegisterationService.getStudentLateCourseInstanceRegistrationRequests(this.student_id_get.value).subscribe(
      requests =>{
        console.log(requests);
      },
      error =>{
        console.log(error.error);
      }
    )
  }

}
