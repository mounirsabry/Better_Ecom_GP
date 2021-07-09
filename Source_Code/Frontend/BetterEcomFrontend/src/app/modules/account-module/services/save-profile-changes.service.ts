import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class SaveProfileChangesService {

  constructor(private httpClient:HttpClient) { }

  saveChanges(user:any){

    return this.httpClient.patch("https://localhost:44361/profile/SaveProfileChanges/" + localStorage.getItem('ID') +"/" + localStorage.getItem('type'), user);
  }

  changePassword(oldPassword : string, newPassword : string)
  {
    return this.httpClient.patch<any>('https://localhost:44361/profile/ChangePassword/' + localStorage.getItem('ID'),{"old_password" : oldPassword, "new_password" : newPassword})
  }
}


