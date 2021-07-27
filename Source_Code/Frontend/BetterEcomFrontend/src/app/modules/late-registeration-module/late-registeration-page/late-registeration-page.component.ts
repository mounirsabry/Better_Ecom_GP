import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LateRegisterationService } from '../services/late-registeration.service';

@Component({
  selector: 'app-late-registeration-page',
  templateUrl: './late-registeration-page.component.html',
  styleUrls: ['./late-registeration-page.component.css']
})
export class LateRegisterationPageComponent implements OnInit {

  CourseTerm = ['First','Second','Summer','Other']
  constructor(private lateRegisterationService: LateRegisterationService) { }

  submitLateCourseRegisterationForm = new FormGroup({
    StudentID: new FormControl,
    CourseInstanceID: new FormControl('', Validators.required)
  })

  deleteLateRegisterationRequestForm = new FormGroup({
    lateRegistrationRequestID: new FormControl('', Validators.required)
  })

  getLateRegisterationRequestsForm = new FormGroup({
    courseInstance: new FormGroup({
      request_id: new FormControl,
      student_id: new FormControl,
      course_instance_id: new FormControl,
      request_date: new FormControl,
      request_status: new FormControl('', Validators.required)
    })
  })

  late_requests_list: Array<any> = []
  available_courses_list: Array<any> = []
  available_coures_instance_list: Array<any> = []
  studentID: number = +localStorage.getItem('ID');
  course_code_list: Array<string> = []
  showAvailableCoursesTable: boolean = false;
  showAvailableInstancesTable: boolean = false;


  ngOnInit(): void {
    this.submitLateCourseRegisterationForm.controls.StudentID.setValue(this.studentID);
    this.late_requests_list = [];
    //console.log(this.student_id_get.value)
    this.getStudentRequests();
  }

  getStudentRequests() {
    this.late_requests_list = []
    this.lateRegisterationService.getStudentLateCourseInstanceRegistrationRequests(this.studentID).subscribe(
      requests => {
        requests.forEach((element, index) => {
          this.late_requests_list.push(element)
        });
        this.getInstanceID();
      },
      error => {
        console.log(error.error);
      }
    )
  }

  getAvailableCourses() {
    this.lateRegisterationService.getStudentAvailableCourses(this.studentID).subscribe(
      data => {
        //this.available_courses_list = data;
        for(let value of Object.values(data)){
          let obj = {}
          for(let subkey of Object.keys(value)){
            console.log(subkey)
            if(subkey != 'is_archived' && subkey != 'is_read_only'){
             // console.log(key)
             obj[subkey] = value[subkey]

            }
          }
          this.available_courses_list.push(obj)

        }
        console.log(this.available_courses_list);
        this.showAvailableCoursesTable = true;
      },
      error => {
        console.log(error.error);
      }
    )
  }

  getAvailableCourseInstances(index: number) {
    var course_code: string = this.available_courses_list[index].course_code;
    this.lateRegisterationService.getCourseAvailableCourseInstances(course_code).subscribe(
      data => {
        if (data.length < 1) { alert("No Course Instances were found") }

        this.available_coures_instance_list = data;

        for(let value of Object.values(this.available_coures_instance_list)){
          for(let key of Object.keys(value)){
            if(key == 'course_term'){
              value[key] = this.CourseTerm[value[key]]
            }
          }
        }
        console.log(this.available_coures_instance_list);
        this.showAvailableInstancesTable = true;
      },
      error => {
        console.log(error.error);
      }
    )
  }

  submitRequest(index: number) {
    console.log(index)
    var instance_id: number = this.available_coures_instance_list[index].instance_id;
    this.submitLateCourseRegisterationForm.controls['CourseInstanceID'].setValue(instance_id);
    console.log(this.submitLateCourseRegisterationForm.value);
    this.lateRegisterationService.submitLateCourseInstanceRegistrationRequest(this.submitLateCourseRegisterationForm.value).subscribe(
      response => {
        this.getStudentRequests();
        alert("Registeration submited");
      },
      error => {
        console.log(error.error);
        alert("failed");
      }
    )
  }

  deleteRequest(index: number) {
    //var request_id : number = this.deleteLateRegisterationRequestForm.controls.lateRegistrationRequestID.value
    var request_id: number = this.late_requests_list[index].request_id;
    this.lateRegisterationService.deleteLateCourseInstanceRegistrationRequest(request_id).subscribe(
      response => {
        this.late_requests_list.splice(index, 1);
        alert("Request Deleted");
        console.log(response);
      },
      error => {
        console.log(error);
      }
    )
  }

  objectValues(obj) {
    return Object.values(obj);
  }

  objectKeys(obj) {
    return Object.keys(obj);
  }

  getInstanceID() {
    this.late_requests_list.forEach((element, index) => {
      this.lateRegisterationService.getCourseInstanceByID(element.course_instance_id).subscribe(
        data => {
          //console.log(data[0].course_code);
          this.late_requests_list[index].course_instance_id = data[0].course_code;
          this.mapRequestStatus(index);
        }
      )
    })
    console.log(this.late_requests_list);
  }

  mapRequestStatus(index: number) {
    if (this.late_requests_list[index].request_status == 0) { this.late_requests_list[index].request_status = 'Pending_Accept' }
    else if (this.late_requests_list[index].request_status == 1) { this.late_requests_list[index].request_status = 'Accepted' }
    else if (this.late_requests_list[index].request_status == 2) { this.late_requests_list[index].request_status = 'Rejected' }

  }
}
