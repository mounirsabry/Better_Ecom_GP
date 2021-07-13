import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UpdateDataAdminService {

  constructor(private httpClient:HttpClient) { }

  createAccount(id : number){
    return this.httpClient.patch<any>('https://localhost:44361/createAccount/' + localStorage.getItem('type'), {"ID" : id})
  }

  resetPassword(user : any){
    return this.httpClient.patch<any>('https://localhost:44361/adminResetPassword', user)
  }
}
