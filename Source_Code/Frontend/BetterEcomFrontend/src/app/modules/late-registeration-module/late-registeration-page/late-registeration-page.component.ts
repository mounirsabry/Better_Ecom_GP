import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LateRegisterationService } from '../services/late-registeration.service';

@Component({
  selector: 'app-late-registeration-page',
  templateUrl: './late-registeration-page.component.html',
  styleUrls: ['./late-registeration-page.component.css']
})
export class LateRegisterationPageComponent implements OnInit {

  constructor(private lateRegisterationService : LateRegisterationService) { }

  submitLateCourseRegisterationForm = new FormGroup({
    StudentID : new FormControl,
    CourseInstanceID : new FormControl('', Validators.required)
  })

  deleteLateRegisterationRequestForm = new FormGroup({
    lateRegistrationRequestID : new FormControl('', Validators.required)
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

  late_requests_list : Array<any> = []
  available_courses_list : Array<any> = []
  studentID : number = +localStorage.getItem('ID');
  course_code_list : Array<string> = []


  ngOnInit(): void {
    this.submitLateCourseRegisterationForm.controls.StudentID.setValue(this.studentID);
    this.late_requests_list = [];
    //console.log(this.student_id_get.value)
    this.lateRegisterationService.getStudentLateCourseInstanceRegistrationRequests(this.studentID).subscribe(
      requests =>{
        requests.forEach((element,index) => {
          this.late_requests_list.push(element)
        });
        this.getInstanceID();
      },
      error =>{
        console.log(error.error);
      }
    )
  }

  getAvailableCourses(){
    this.lateRegisterationService.getStudentAvailableCourses(this.studentID).subscribe(
      data =>{
        this.available_courses_list = data;
        console.log(this.available_courses_list);
      },
      error =>{
        console.log(error.error);
      }
    )
  }

  submitRequest(){
    this.lateRegisterationService.submitLateCourseInstanceRegistrationRequest(this.submitLateCourseRegisterationForm.value).subscribe(
      response =>{
        alert("Registeration submited");
      },
      error =>{
        console.log(error.error);
        alert("failed");
      }
    )
  }

  deleteRequest(index : number){
    //var request_id : number = this.deleteLateRegisterationRequestForm.controls.lateRegistrationRequestID.value
    var request_id : number = this.late_requests_list[index].request_id;
    this.lateRegisterationService.deleteLateCourseInstanceRegistrationRequest(request_id).subscribe(
      response =>{
        this.late_requests_list.splice(index, 1);
        alert("Request Deleted");
        console.log(response);
      },
      error =>{
        console.log(error);
      }
    )
  }

  /*getStudentRequests(){
    this.late_requests_list = [];
    //console.log(this.student_id_get.value);
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
  }*/

  objectValues(obj){
    return Object.values(obj);
  }

  objectKeys(obj){
    return Object.keys(obj);
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
    console.log(this.late_requests_list);
  }

  mapRequestStatus(index : number){
    if(this.late_requests_list[index].request_status == 0) {this.late_requests_list[index].request_status = 'Pending_Accept'}
    else if (this.late_requests_list[index].request_status == 1) {this.late_requests_list[index].request_status = 'Accepted'}
    else if (this.late_requests_list[index].request_status == 2) {this.late_requests_list[index].request_status = 'Rejected'}

  }
}
