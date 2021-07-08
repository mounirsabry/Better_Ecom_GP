import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class SaveProfileChangesService {

  constructor(private httpClient:HttpClient) { }

  saveChanges(user:any){

    this.httpClient.patch("",user);
  }

  changePassword(oldPassword : string, newPassword : string)
  {
    return this.httpClient.patch<any>('',{"oldPassword" : oldPassword, "newPassword" : newPassword})
  }
}


