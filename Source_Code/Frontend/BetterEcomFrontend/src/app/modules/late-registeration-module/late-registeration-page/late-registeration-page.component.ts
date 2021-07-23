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

}
