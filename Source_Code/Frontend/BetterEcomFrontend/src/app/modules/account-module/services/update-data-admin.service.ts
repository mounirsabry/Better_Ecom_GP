import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UpdateDataAdminService {

  constructor(private httpClient:HttpClient) { }

  createAccount(account : any){
    return this.httpClient.patch<any>('', account)
  }

  resetPassword(user : any){
    return this.httpClient.patch<any>('', user)
  }
}
