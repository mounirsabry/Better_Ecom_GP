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

  updateCoursePrerequisiteInfo(coursePrerequisite : any){
    return this.httpclient.patch<any>("https://localhost:44361/department/UpdateCoursePrerequisites", coursePrerequisite);
  }

  updateCourseDepartmentApplicabilityInfo(courseDepApp : any){
    return this.httpclient.patch<any>("https://localhost:44361/department/UpdateCourseDepartmentApplicability", courseDepApp);
  }
  
  archiveCourse(course : any){
    return this.httpclient.delete<any>("https://localhost:44361/department/ArchiveCourse/"+ course);
  }

  handleError(error : HttpErrorResponse){
    return throwError(error);
  }
}
