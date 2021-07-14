import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UpdateDataAdminService {

  constructor(private httpClient:HttpClient) { }

  createAccount(id : number, type:string){
    let idToSend = (type == 'student')? 'StudentID' : 'InstructorID'

    let idObj = {}

    idObj[idToSend] = id
    return this.httpClient.post<any>('https://localhost:44361/account/createAccountFor' + type, idObj)
  }

  resetPassword(user : any){
    return this.httpClient.patch<any>('https://localhost:44361/account/ResetAccountCredientials', user)
  }
}
