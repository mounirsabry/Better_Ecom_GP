import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CourseInstanceService {

  constructor(private httpclient:HttpClient) { }

  getAvailabelTerms(){
    return this.httpclient.get<any>("https://localhost:44361/department/GetAvailableTerms");
  }

  getCourseInstanceFromCourse(course_code : string){
    return this.httpclient.get<any>("https://localhost:44361/department/GetCourseInstancesFromCourse/" + course_code);
  }

  getCourseInstanceByID(instance_id : number){
    return this.httpclient.get<any>("https://localhost:44361/department/GetCourseInstanceByID/" + instance_id);
  }

  addCourseInstance(course_instance : any){

    console.log(course_instance)
    return this.httpclient.post<any>("https://localhost:44361/department/AddCourseInstance" , course_instance);
  }


  getStudentCourseInstanceGrade(student_id : number ,course_instance_id : number){
    return this.httpclient.get<any>("https://localhost:44361/Grade/GetStudentCourseInstanceGrade/" + student_id + "/" + course_instance_id);
  }

  setStudentCourseInstanceGrade(student : any){
    return this.httpclient.patch<any>("https://localhost:44361/Grade/SetStudentCourseInstanceGrade", student);
  }

  getStudentGPA(student_id : number){
    return this.httpclient.get<any>("https://localhost:44361/Grade/GetStudentGPA/" + student_id);
  }

  handleError(error : HttpErrorResponse){
    return throwError(error);
  }
}
