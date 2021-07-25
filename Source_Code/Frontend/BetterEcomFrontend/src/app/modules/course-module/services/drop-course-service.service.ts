import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class DropCourseServiceService {

  constructor(private httpclient: HttpClient) { }

  dropStudentFromCourseInstance(course_instance_id : number, student_id: number) {
    return this.httpclient.delete<any>("https://localhost:44361/course/DropStudentFromCourseInstance/" + course_instance_id + "/" + student_id);
  }

  dropInstructorFromCourseInstance(course_instance_id : number, instructor_id: number) {
    return this.httpclient.delete<any>("https://localhost:44361/course/DropInstructorFromCourseInstance/" + course_instance_id + "/" + instructor_id);
  }

  handleError(error : HttpErrorResponse){
    return throwError(error);
  }

}
