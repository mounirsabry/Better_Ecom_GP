import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ViewRegisteredCoursesService {


  constructor(private httpclient:HttpClient) { }

  getStudentRegisteredCourses(student_id : number){
    return this.httpclient.get<any>("https://localhost:44361/course/GetStudentRegisteredCourses/" + student_id);
  }

  getStudentRegisteredCourseInstances(student_id : number){
    return this.httpclient.get<any>("https://localhost:44361/course/GetStudentRegisteredCourseInstances/" + student_id);
  }

  GetCourseStudentRegisteredCourseInstances(student_id : number, course_code : string){
    return this.httpclient.get<any>("https://localhost:44361/course/GetCourseStudentRegisteredCourseInstances/" + student_id + "/" + course_code);
  }


  getInstructorRegisteredCourses(instructor_id : number){
    return this.httpclient.get<any>("https://localhost:44361/course/GetInstructorRegisteredCourses/" + instructor_id);
  }

  getInstructorRegisteredCourseInstances(instructor_id : number){
    return this.httpclient.get<any>("https://localhost:44361/course/GetInstructorRegisteredCourseInstances/" + instructor_id);
  }

  getCourseInstructorRegisteredCourseInstances(instructor_id : number, course_code : string){
    return this.httpclient.get<any>("https://localhost:44361/course/GetCourseInstructorRegisteredCourseInstances/" + instructor_id + "/" + course_code);
  }

  handleError(error : HttpErrorResponse){
    return throwError(error);
  }
}
