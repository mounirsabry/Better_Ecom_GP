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

  instanceId:number
  registerStudentCourseForm = new FormGroup({
    CourseCode: new FormControl('',[Validators.required]),
    CurrentTerm : new FormControl('',[Validators.required])
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

    this.registerStdInsCourseService.getCourseInstance(this.registerStudentCourseForm.value['CourseCode']).subscribe(
      response =>{
        let currentYear = new Date().getFullYear()

        // for some reason, the enum in db current_term, returns number intead of string,
        //i.e 0 instead of first, 1 instead of second etc.
        let TermToNumber:number

        if(this.registerStudentCourseForm.value['CurrentTerm'] == 'First'){

          TermToNumber = 0
        }else if(this.registerStudentCourseForm.value['CurrentTerm'] == 'Second'){

          TermToNumber = 1

        }else{

          TermToNumber = 2
        }

        for(let key of Object.keys(response)){

          if(response[key]['course_year'] == currentYear && response[key]['course_term'] == TermToNumber ){
            this.instanceId = response[key]['instance_id']
          }
        }

        let idsObj = {}
        idsObj['StudentID'] = this.registerStudentCourseForm.value['StudentID']
        idsObj['CourseInstanceID'] = this.instanceId
        this.registerStdInsCourseService.registerInCourse(this.type,idsObj).subscribe(

          response => {
            alert('registeration Successful!')
          },
          error => {
            alert('registeration Failed!')


          }
        )


      },


      error => {
        alert('Registeration Failed!')
      }
    )

    /*this.registerStdInsCourseService.registerInCourse(this.type,this.registerStudentCourseForm.value).subscribe(

      response => {
        alert("registeration Successfull!")
      },
      error =>{
        alert("registeration Failed!")
      }
    )*/

  }

  get GetInstructorID(){
    return this.registerStudentCourseForm.get('InstructorID')
  }


  get GetStudentID(){
    return this.registerStudentCourseForm.get('StudentID')
  }

  get GetCourseCode(){
    return this.registerStudentCourseForm.get('CourseCode')
  }

  get GetCurrentTerm(){
    return this.registerStudentCourseForm.get('CurrentTerm')
  }

}
