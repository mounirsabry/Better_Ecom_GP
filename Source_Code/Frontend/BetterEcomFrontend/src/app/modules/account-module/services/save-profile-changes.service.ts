import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class SaveProfileChangesService {

  constructor(private httpClient:HttpClient) { }

  saveChanges(user:any){

    console.log(user)
    return this.httpClient.patch("https://localhost:44361/profile/SaveProfileChanges", user);
  }

  changePassword(oldPassword : string, newPassword : string)
  {
    return this.httpClient.patch<any>('https://localhost:44361/profile/ChangePassword' ,{"OldPassword" : oldPassword, "NewPassword" : newPassword})
  }
}


