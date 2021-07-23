import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { RegisterStudentInstructorCourseService } from '../services/register-student-instructor-course.service';

@Component({
  selector: 'app-register-student-instructor-in-a-course',
  templateUrl: './register-student-instructor-in-a-course.component.html',
  styleUrls: ['./register-student-instructor-in-a-course.component.css']
})
export class RegisterStudentInstructorInACourseComponent implements OnInit {

  type:string

  registerStudentCourseForm = new FormGroup({
    CourseInstanceID: new FormControl('',[Validators.required]),
  })
  constructor(private activatedRoute:ActivatedRoute,
              private registerStdInsCourseService:RegisterStudentInstructorCourseService) { }

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params: ParamMap) => {
      this.type = params.get('type')
      console.log(this.type)
    })

    if(this.type == 'student')
      this.registerStudentCourseForm.addControl('StudentID',new FormControl('',[Validators.required]))
    else
      this.registerStudentCourseForm.addControl('InstructorID',new FormControl('',[Validators.required]))


  }

  register(){

    this.registerStdInsCourseService.registerInCourse(this.type,this.registerStudentCourseForm.value).subscribe(

      response => {
        alert("registeration Successfull!")
      },
      error =>{
        alert("registeration Failed!")
      }
    )

  }

  get GetInstructorID(){
    return this.registerStudentCourseForm.get('InstructorID')
  }


  get GetStudentID(){
    return this.registerStudentCourseForm.get('StudentID')
  }

  get GetCourseInstanceID(){
    return this.registerStudentCourseForm.get('CourseInstanceID')
  }


}
