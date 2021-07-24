import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RegisterStudentInstructorCourseService {

  constructor(private httpClient:HttpClient) { }

  registerInCourse(type:string, IDsObj:any){

    if(type == 'student')
      return this.httpClient.post("https://localhost:44361/Course/RegisterToCourseInstance",IDsObj)
    else
      return this.httpClient.post("https://localhost:44361/Course/RegisterInstructorToCourseInstance",IDsObj)
  }

  getCourseInstance(courseCode:string){
    return this.httpClient.get("https://localhost:44361/department/GetCourseInstancesFromCourse/"+courseCode)
  }
}
