import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http'
import {catchError} from 'rxjs/operators'
import { throwError } from 'rxjs';
import { ParseSpan } from '@angular/compiler';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private httClient:HttpClient) { }


  login(ID:number, password:string, type:string) {

    let user:any = {
      "UserID":ID,
      "Password":password,
      "Type":type
    }

    // to use the ID in view profile component.
    localStorage.setItem('ID', ID +'');
    localStorage.setItem('type', type);
    return this.httClient.post<any>('https://localhost:44361/account/login',user)

  }

}
