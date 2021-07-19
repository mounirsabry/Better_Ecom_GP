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
}
