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

  ngOnInit(): void {
    this.submitLateCourseRegisterationForm.controls.StudentID.setValue(+localStorage.getItem('ID'));
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

  deleteRequest(){
    var request_id = this.deleteLateRegisterationRequestForm.controls.lateRegistrationRequestID.value
    this.lateRegisterationService.deleteLateCourseInstanceRegistrationRequest(request_id).subscribe(
      response =>{
        alert("Request Deleted");
        console.log(response);
      },
      error =>{
        console.log(error);
      }
    )
  }

}
