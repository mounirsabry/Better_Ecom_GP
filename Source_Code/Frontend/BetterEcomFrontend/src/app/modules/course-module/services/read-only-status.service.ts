import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReadOnlyStatusService {

  constructor(private httpclient: HttpClient) { }

  
  getCourseInstanceReadOnlyStatus(course_instance_id : number) {
    return this.httpclient.get<any>("https://localhost:44361/course/GetCourseInstanceReadOnlyStatus/" + course_instance_id);
  }

  setCourseInstanceReadOnlyStatus(course_instance_id : number, readOnlyStatus : boolean) {
    var obj = {
      'CourseInstanceID' : course_instance_id,
      'ReadOnlyStatus' : readOnlyStatus
    }
    return this.httpclient.patch<any>("https://localhost:44361/course/SetCourseInstanceReadOnlyStatus", obj);
  }

  handleError(error : HttpErrorResponse){
    return throwError(error);
  }
}
