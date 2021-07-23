import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LateRegisterationService {

  constructor(private httpclient:HttpClient) { }

  getAllLateCourseRegistrationRequests(){ //admin
    return this.httpclient.get<any>("https://localhost:44361/course/GetAllLateCourseInstanceRegistrationRequests");
  }

  getCourseLateCourseRegistrationRequests(course_code : string){ //admin
    return this.httpclient.get<any>("https://localhost:44361/course/GetCourseLateCourseRegistrationRequests" + course_code);
  }

  getStudentLateCourseInstanceRegistrationRequests(student_id : string){ //student, admin
    return this.httpclient.get<any>("https://localhost:44361/course/GetStudentLateCourseInstanceRegistrationRequests" + student_id); 
  }

  getLateCourseInstanceRegistrationRequestAvailableStatus(){  //admin
    return this.httpclient.get<any>("https://localhost:44361/course/GetLateCourseInstanceRegistrationRequestAvailableStauts");
  }

  submitLateCourseInstanceRegistrationRequest(request : any){ //student
    return this.httpclient.post<any>("https://localhost:44361/course/SubmitLateCourseInstanceRegistrationRequest", request);
  }

  deleteLateCourseInstanceRegistrationRequest(request : any){ //student
    return this.httpclient.delete<any>("https://localhost:44361/course/DeleteLateCourseInstanceRegistrationRequest", request);
  }

  setLateCourseInstanceRegistrationRequest(request : any){ //admin
    return this.httpclient.delete<any>("https://localhost:44361/course/SetLateCourseInstanceRegistrationRequest", request);
  }

  getCourseInstancesFromCourse(course_code : string){
    return this.httpclient.get<any>("https://localhost:44361/department/GetCourseInstancesFromCourse/" + course_code); 
  }

  getCourseInstanceByID(instance_id : number){
    return this.httpclient.get<any>("https://localhost:44361/department/GetCourseInstanceByID/" + instance_id); 
  }


  handleError(error : HttpErrorResponse){
    return throwError(error);
  }
}
