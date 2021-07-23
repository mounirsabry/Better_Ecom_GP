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
    this.late_requests_list = [];
    this.lateRegisterationService.getAllLateCourseRegistrationRequests().subscribe(
      requests =>{
        requests.forEach((element,index) => {
          this.late_requests_list.push(element)
        });
        console.log(this.late_requests_list);
      },
      error =>{
        console.log(error.error);
      }
    )
  }

  getCourseRequests(){
    this.late_requests_list = [];
    console.log(this.course_code_get.value);
    this.lateRegisterationService.getCourseLateCourseRegistrationRequests(this.course_code_get.value).subscribe(
      requests =>{
        requests.forEach((element,index) => {
          this.late_requests_list.push(element)
        });
        console.log(this.late_requests_list);
      },
      error =>{
        console.log(error.error);
      }
    )
  }

  getStudentRequests(){
    this.late_requests_list = [];
    console.log(this.student_id_get.value);
    var student_id : number = +this.student_id_get.value;
    this.lateRegisterationService.getStudentLateCourseInstanceRegistrationRequests(student_id).subscribe(
      requests =>{
        requests.forEach((element,index) => {
          this.late_requests_list.push(element)
        });
        console.log(this.late_requests_list);
      },
      error =>{
        console.log(error.error);
      }
    )
  }

  objectValues(obj){
    return Object.values(obj);
  }

  objectKeys(obj){
    return Object.keys(obj);
  }

  acceptRequest(index : number){
    console.log(this.late_requests_list[index]);
    var setRequest = {
      'RequestID' : this.late_requests_list[index].request_id,
      'RequestStatus' : 1
    }

    this.lateRegisterationService.setLateCourseInstanceRegistrationRequest(setRequest).subscribe(
      response =>{
        this.late_requests_list.splice(index, 1);
        alert("Request accepted");
      },
      error =>{
        alert(error.error);
      }
    )
  }

  rejectRequest(index : number){
    console.log(this.late_requests_list[index]);
    var setRequest = {
      'RequestID' : this.late_requests_list[index].request_id,
      'RequestStatus' : 2
    }

    this.lateRegisterationService.setLateCourseInstanceRegistrationRequest(setRequest).subscribe(
      response =>{
        this.late_requests_list.splice(index, 1);
        alert("Request rejected");
      },
      error =>{
        alert(error.error);
      }
    )
  }

  checkIfAcceptedOrRejected(index : number){
    if(this.late_requests_list[index].request_status == 1 || this.late_requests_list[index].request_status == 2){
      return true;
    }
    else {return false;}
  }
}
