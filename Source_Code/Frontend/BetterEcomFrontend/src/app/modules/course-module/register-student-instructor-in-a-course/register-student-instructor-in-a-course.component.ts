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

  instanceId:number = -2
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


        console.log(this.instanceId)
        if(this.instanceId != -2){
          let idsObj = {}
          if(this.type == 'student')
            idsObj['StudentID'] = this.registerStudentCourseForm.value['StudentID']
          else
            idsObj['InstructorID'] = this.registerStudentCourseForm.value['InstructorID']


          idsObj['CourseInstanceID'] = this.instanceId
          this.registerStdInsCourseService.registerInCourse(this.type,idsObj).subscribe(

            response => {
              alert('registeration Successful!')
            },
            error => {

              alert('registeration Failed!'+ ((this.type == 'student')?'The Registeration Time could be over, or ':'')+ this.type + 'ID is not registered in the faculty or ' + this.type + 'ID is already registered in this course')

            }
            )
        }else{
          alert('no course was found with the specified info!')
        }



      },


      error => {
        alert('Registeration Failed! server may be down')
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
