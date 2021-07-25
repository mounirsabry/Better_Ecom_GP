import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LateRegisterationService {

  constructor(private httpclient: HttpClient) { }

  getStudentAvailableCourses(student_id: number) {
    return this.httpclient.get<any>("https://localhost:44361/course/GetStudentAvailableCourses/" + student_id);
  }

  getIsNormalCourseRegistrationOpen() { //admin, student
    return this.httpclient.get<any>("https://localhost:44361/course/GetIsNormalCourseRegistrationOpen");
  }

  getIsLateCourseRegistrationOpen() { //admin, student
    return this.httpclient.get<any>("https://localhost:44361/course/GetIsLateCourseRegistrationOpen");
  }

  getIsDropCourseRegistrationOpen() { //admin, student
    return this.httpclient.get<any>("https://localhost:44361/course/GetIsDropCourseRegistrationOpen");
  }

  getAllLateCourseRegistrationRequests() { //admin
    return this.httpclient.get<any>("https://localhost:44361/course/GetAllLateCourseInstanceRegistrationRequests");
  }

  getCourseAvailableCourseInstances(course_code: string) {
    return this.httpclient.get<any>("https://localhost:44361/course/GetCourseAvailableCourseInstances/" + course_code);
  }


  getCourseLateCourseRegistrationRequests(course_code: string) { //admin
    return this.httpclient.get<any>("https://localhost:44361/course/GetCourseLateCourseRegistrationRequests/" + course_code);
  }

  getStudentLateCourseInstanceRegistrationRequests(student_id: number) { //student, admin
    return this.httpclient.get<any>("https://localhost:44361/course/GetStudentLateCourseInstanceRegistrationRequests/" + student_id);
  }

  getLateCourseInstanceRegistrationRequestAvailableStatus() {  //admin
    return this.httpclient.get<any>("https://localhost:44361/course/GetLateCourseInstanceRegistrationRequestAvailableStauts");
  }

  submitLateCourseInstanceRegistrationRequest(request: any) { //student
    return this.httpclient.post<any>("https://localhost:44361/course/SubmitLateCourseInstanceRegistrationRequest", request);
  }

  deleteLateCourseInstanceRegistrationRequest(request_id: number) { //student
    return this.httpclient.delete<any>("https://localhost:44361/course/DeleteLateCourseInstanceRegistrationRequest/" + request_id);
  }

  setLateCourseInstanceRegistrationRequest(request: any) { //admin
    return this.httpclient.patch<any>("https://localhost:44361/course/SetLateCourseInstanceRegistrationRequest", request);
  }

  getCourseInstancesFromCourse(course_code: string) {
    return this.httpclient.get<any>("https://localhost:44361/department/GetCourseInstancesFromCourse/" + course_code);
  }

  getCourseInstanceByID(instance_id: number) {
    return this.httpclient.get<any>("https://localhost:44361/department/GetCourseInstanceByID/" + instance_id);
  }


  dropStudent() { }
  dropInstructor() { }
  setCourseInstanceReadOnly() { }

  handleError(error: HttpErrorResponse) {
    return throwError(error);
  }
}
