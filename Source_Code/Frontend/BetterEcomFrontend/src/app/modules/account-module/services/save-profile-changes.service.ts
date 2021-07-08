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

export function passwordValidator(control:AbstractControl): { [key :string] : boolean} {
    const newPassword = control.get('newPassword');
    const confirmPassword = control.get('confirmPassword');
    if (newPassword.pristine || confirmPassword.pristine)
    {
      return null;
    }
    if(newPassword && confirmPassword && newPassword.value != confirmPassword.value)
    {
      console.log("hey")
    }
    return newPassword && confirmPassword && newPassword.value != confirmPassword.value ?
      {'misMatch' : true} :
      null;
}
