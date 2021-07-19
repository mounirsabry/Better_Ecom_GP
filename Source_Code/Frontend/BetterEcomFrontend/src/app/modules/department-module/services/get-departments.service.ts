import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class GetDepartmentsService {

  constructor(private httpclient:HttpClient) { }

  getDepartmentsData(){
    return this.httpclient.get<any>("https://localhost:44361/department/GetDepartments")
  }
}
