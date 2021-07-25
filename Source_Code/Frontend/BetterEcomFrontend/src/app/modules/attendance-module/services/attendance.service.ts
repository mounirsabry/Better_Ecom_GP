import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AttendanceService {

  constructor(private httpClient: HttpClient) { }

  addAttendanceItem(attendanceItem:any){

    return this.httpClient.post("https://localhost:44361/Attendance/AddCourseInstanceAttendanceItem",attendanceItem)
  }

  getAttendanceInfo(intanceID:number,studentID:number){

    return this.httpClient.get<any>("https://localhost:44361/Attendance/GetStudentCourseInstanceAttendance/"+intanceID+"/"+studentID)
  }
}
