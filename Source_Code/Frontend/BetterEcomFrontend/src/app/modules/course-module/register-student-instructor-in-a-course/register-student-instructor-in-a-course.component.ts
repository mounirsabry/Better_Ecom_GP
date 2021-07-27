import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { LateRegisterationService } from '../../late-registeration-module/services/late-registeration.service';
import { RegisterStudentInstructorCourseService } from '../services/register-student-instructor-course.service';

@Component({
  selector: 'app-register-student-instructor-in-a-course',
  templateUrl: './register-student-instructor-in-a-course.component.html',
  styleUrls: ['./register-student-instructor-in-a-course.component.css']
})
export class RegisterStudentInstructorInACourseComponent implements OnInit {

  type:string

  logedInUser:string

  CourseTerm = ['First','Second','Summer','Other']
  availableCourses : Array<any> = []
  availableCouresInstances : Array<any> = []
  showAvailableInstancesTable = false

  instanceId:number = -2
  registerStudentCourseForm = new FormGroup({
    CourseCode: new FormControl('',[Validators.required]),
    CurrentTerm : new FormControl('',[Validators.required])
  })
  constructor(private activatedRoute:ActivatedRoute,
              private registerStdInsCourseService:RegisterStudentInstructorCourseService,
              private registerService: LateRegisterationService) { }

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params: ParamMap) => {
      this.type = params.get('type')


    })



    this.logedInUser = localStorage.getItem('type')
    if(this.type == 'student')
      this.registerStudentCourseForm.addControl('StudentID',new FormControl('',[Validators.required]))
    else
      this.registerStudentCourseForm.addControl('InstructorID',new FormControl('',[Validators.required]))


      this.registerService.getStudentAvailableCourses(parseInt(localStorage.getItem('ID'))).subscribe(
        response =>{
          //this.availableCourses = response;


          for(let value of Object.values(response)){
            let obj = {}
            for(let subkey of Object.keys(value)){
                console.log(subkey)
                if(subkey != 'is_archived' && subkey != 'is_read_only'){
                // console.log(key)
                obj[subkey] = value[subkey]

              }
          }
          this.availableCourses.push(obj)

        }


          console.log(this.availableCourses)
        },
        error =>{


        }
      )

  }

  registerIfUserStudent(index :number){
    var instance_id : number = this.availableCouresInstances[index].instance_id;

    let idsObj = {}

    idsObj['StudentID'] = parseInt(localStorage.getItem('ID'))
    idsObj['CourseInstanceID'] = instance_id

    console.log('instance id')
    console.log(instance_id)

    this.registerStdInsCourseService.registerInCourse(this.type,idsObj).subscribe(

      response => {
        alert('registeration Successful!')
      },
      error => {

        alert('registeration Failed! The Registeration Time could be over, or you are already registered in this course')

      }
      )

  }
  register(){

    this.registerStdInsCourseService.getCourseInstance(this.registerStudentCourseForm.value['CourseCode']).subscribe(
      response =>{

        let currentYear = new Date().getFullYear()

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

  objectValues(obj){
    return Object.values(obj);
  }

  getAvailableCourseInstances(courseIndex){
    var course_code : string = this.availableCourses[courseIndex].course_code;
    this.registerService.getCourseAvailableCourseInstances(course_code).subscribe(
      data =>{
        if(data.length < 1) {alert("No Course Instances were found")}

        this.availableCouresInstances.push(...data) ;

        for(let value of Object.values(this.availableCouresInstances)){
          for(let key of Object.keys(value)){
            if(key == 'course_term'){
              value[key] = this.CourseTerm[value[key]]
            }
          }
        }
        this.showAvailableInstancesTable = true;
      },
      error =>{
        console.log(error.error);
      }
    )

  }

}
/*
<table style="text-align: center;" [hidden]="!showAvailableCoursesTable">
  <tr>
      <th></th> <!--remove deh ya mina lw na ma sheltha4-->
      <th>Academic Year</th>
      <th>Course Code</th>
      <th>Course Description</th>
      <th>Course Name</th>
      <th>Department Code</th>
      <th>Is Archived</th>
      <th>Is Read only</th>
  </tr>

  <tr *ngFor="let courses of objectValues(available_courses_list);let courseIndex = index">

      <td>{{ courseIndex }}</td>
      <td *ngFor="let course of courses | keyvalue">{{ course.value }}</td>
      <td><button type="button" (click)="getAvailableCourseInstances(courseIndex)">Select</button></td>

  </tr>

</table>


<table style="text-align: center;" [hidden]="!showAvailableInstancesTable">
  <tr>
      <th></th> <!--remove deh ya mina lw na ma sheltha4-->
      <th>Course Code</th>
      <th>Course Term</th>
      <th>Course Year</th>
      <th>Credit Hours</th>
      <th>Instance ID</th>
  </tr>

  <tr *ngFor="let instances of objectValues(available_coures_instance_list);let instanceIndex = index">

      <td>{{ instanceIndex }}</td>
      <td *ngFor="let instance of instances | keyvalue">{{ instance.value }}</td>
      <td><button type="button" (click)="submitRequest(instanceIndex)">Register in Course</button></td>

  </tr>

</table>

*/
