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
  constructor(private attendacneService:AttendanceService) { }

  attendanceForm = new FormGroup({
    AttendanceType: new FormControl('',Validators.required),
    ItemName: new FormControl('',Validators.required),
    AttendanceDate: new FormControl('',Validators.required),


  })
  logedInType:string = localStorage.getItem('type')
  ngOnInit(): void {
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



}
