import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AttendanceService } from '../services/attendance.service';

@Component({
  selector: 'app-attendance-page',
  templateUrl: './attendance-page.component.html',
  styleUrls: ['./attendance-page.component.css']
})
export class AttendancePageComponent implements OnInit {


  @Input() public instanceID

  viewAttendanceForm = new FormGroup({
    StudentID : new FormControl('',Validators.required)
  })

  get getStudentID(){
    return this.viewAttendanceForm.get('StudentID')
  }
  constructor(private attendacneService:AttendanceService) { }

  attendanceForm = new FormGroup({
    AttendanceType: new FormControl('',Validators.required),
    ItemName: new FormControl('',Validators.required),
    AttendanceDate: new FormControl('',Validators.required),


  })
  logedInType:string = localStorage.getItem('type')

   attendanceTypes = ['Lab','Section']
   attendanceStatus = ['Attended','Absent','Excused','Not_Specified']
  attendanceInfoList:Array<any> = []


  ngOnInit(): void {

    if(localStorage.getItem('type') == 'student'){
      this.attendacneService.getAttendanceInfo(parseInt(this.instanceID),parseInt(localStorage.getItem('ID'))).subscribe(
        response =>{
          this.attendanceInfoList = response

          // transform numbers to there meaning.
          for(let key of Object.keys(this.attendanceInfoList)){
            for(let subkey of Object.keys(this.attendanceInfoList[key])){

              if(subkey == 'attendance_type'){
               this.attendanceInfoList[key][subkey] = this.attendanceTypes[this.attendanceInfoList[key][subkey]]
              }else if(subkey == 'attendance_status'){
                this.attendanceInfoList[key][subkey] = this.attendanceStatus[this.attendanceInfoList[key][subkey]]

              }

            }
          }
        },
        error =>{

        }
      )
    }
  }

  addAttendaceItem(){

   let attendacneItem ={}

   attendacneItem = this.attendanceForm.value



   attendacneItem['CourseInstanceID'] = parseInt(this.instanceID)

   // as in back it is required to be int
   if(attendacneItem['AttendanceType'] == 'Lab')
      attendacneItem['AttendanceType'] = 0
    else
      attendacneItem['AttendanceType'] = 1

    console.log(attendacneItem)


   this.attendacneService.addAttendanceItem(attendacneItem).subscribe(
     response =>{

      alert('item added successfully!')
      location.reload()
     },
     error =>{
      alert('Failed! the attendance item maybe already added Or You are not registered in this course')


     }
   )

  }

  get getAttendanceType(){
    return this.attendanceForm.get('AttendanceType')
  }

  get getItemName(){
    return this.attendanceForm.get('ItemName')
  }


  get getAttendanceDate(){
    return this.attendanceForm.get('AttendanceDate')
  }

  objectValues(availableCourses){
    return Object.values(availableCourses)
  }

  setStatus(){

  }

  deleteItem(){

  }
  viewStudentAttendance(){

    this.attendacneService.getAttendanceInfo(parseInt(this.instanceID),this.viewAttendanceForm.value['StudentID']).subscribe(
      response =>{

        this.attendanceInfoList = response

          // transform numbers to there meaning.
          for(let key of Object.keys(this.attendanceInfoList)){
            for(let subkey of Object.keys(this.attendanceInfoList[key])){

              if(subkey == 'attendance_type'){
               this.attendanceInfoList[key][subkey] = this.attendanceTypes[this.attendanceInfoList[key][subkey]]
              }else if(subkey == 'attendance_status'){
                this.attendanceInfoList[key][subkey] = this.attendanceStatus[this.attendanceInfoList[key][subkey]]

              }

            }
          }


      },
      error =>{

      }
    )
  }

}
