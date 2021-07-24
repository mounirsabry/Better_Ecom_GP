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
  showTable : boolean = false;

  ngOnInit(): void {
  }

  getAllRequests(){
    this.showTable = false;
    this.late_requests_list = [];
    this.lateRegisterationService.getAllLateCourseRegistrationRequests().subscribe(
      requests =>{
        requests.forEach((element,index) => {
          if(!this.checkIfAcceptedOrRejected(element))
            this.late_requests_list.push(element)

        });
        if(this.late_requests_list.length < 1) {alert('There is no new requests');}
        this.getInstanceID();
        console.log(this.late_requests_list);
        this.showTable = true;
      },
      error =>{
        console.log(error.error);
      }
    )
  }

  getCourseRequests(){
    this.showTable = false;
    this.late_requests_list = [];
    console.log(this.course_code_get.value);
    this.lateRegisterationService.getCourseLateCourseRegistrationRequests(this.course_code_get.value).subscribe(
      requests =>{
        requests.forEach((element,index) => {
          if(!this.checkIfAcceptedOrRejected(element))
            this.late_requests_list.push(element)
        });
        if(this.late_requests_list.length < 1) {alert('There is no new requests');}
        this.getInstanceID();
        console.log(this.late_requests_list);
        this.showTable = true;
      },
      error =>{
        console.log(error.error);
      }
    )
  }

  getStudentRequests(){
    this.showTable = false;
    this.late_requests_list = [];
    console.log(this.student_id_get.value);
    var student_id : number = +this.student_id_get.value;
    this.lateRegisterationService.getStudentLateCourseInstanceRegistrationRequests(student_id).subscribe(
      requests =>{
        requests.forEach((element,index) => {
          if(!this.checkIfAcceptedOrRejected(element))
            this.late_requests_list.push(element)
        });
        if(this.late_requests_list.length < 1) {alert('There is no new requests');}
        this.getInstanceID();
        console.log(this.late_requests_list);
        this.showTable = true;
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

  checkIfAcceptedOrRejected(lateRequest){
    if(lateRequest.request_status == 1 || lateRequest.request_status == 2){
      return true;
    }
    else {return false;}
  }


  getInstanceID(){
    this.late_requests_list.forEach((element, index) =>{
      this.lateRegisterationService.getCourseInstanceByID(element.course_instance_id).subscribe(
        data =>{
          //console.log(data[0].course_code);
          this.late_requests_list[index].course_instance_id = data[0].course_code;
          this.mapRequestStatus(index);
        }
      )
    })
  }

  mapRequestStatus(index : number){
    if(this.late_requests_list[index].request_status == 0) {this.late_requests_list[index].request_status = 'Pending Accept'}
    else if (this.late_requests_list[index].request_status == 1) {this.late_requests_list[index].request_status = 'Accepted'}
    else if (this.late_requests_list[index].request_status == 2) {this.late_requests_list[index].request_status = 'Rejected'}

  }
}
