import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DepartmentCoursesService {

  constructor(private httpclient:HttpClient) { }

  getDepartmentCourses(departmentCode : string){
    return this.httpclient.get<any>("https://localhost:44361/department/GetDepartmentCourses/" + departmentCode);
  }

  getCourseInfoByCode(courseCode : string){
    return this.httpclient.get<any>("https://localhost:44361/department/GetCourseInfoByCode/" + courseCode);
  }

  getCourseInfoByName(courseName : string){
    return this.httpclient.get<any>("https://localhost:44361/department/GetCourseInfoByName/" + courseName);
  }

  addCourse(course : any){
    return this.httpclient.post<any>("https://localhost:44361/department/AddCourseToDepartment", course);
  }

  updateCourseInformation(course : any){
    return this.httpclient.patch<any>("https://localhost:44361/department/UpdateCourseInfo", course);
  }

  handleError(error : HttpErrorResponse){
    return throwError(error);
  }
}
