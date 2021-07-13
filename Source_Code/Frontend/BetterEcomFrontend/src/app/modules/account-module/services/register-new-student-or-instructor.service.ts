import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RegisterNewStudentOrInstructorService {

  constructor(private httpClient:HttpClient) { }

  register(type:string,user){
    if(type == 'student')
      return this.httpClient.post("https://localhost:44361/userRegistration/AddNewStudent",user)
    else
      return this.httpClient.post("https://localhost:44361/userRegistration/AddNewInstructor",user)

  }
}
