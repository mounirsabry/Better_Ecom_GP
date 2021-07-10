import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UpdateDataAdminService {

  constructor(private httpClient:HttpClient) { }

  createAccount(id : number){
    return this.httpClient.patch<any>('', {"ID" : id})
  }

  resetPassword(user : any){
    return this.httpClient.patch<any>('', user)
  }
}
