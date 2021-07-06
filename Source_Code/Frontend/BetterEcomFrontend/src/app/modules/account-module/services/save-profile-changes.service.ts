import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SaveProfileChangesService {

  constructor(private httpClient:HttpClient) { }

  saveChanges(user:any){

    this.httpClient.patch("",user);
  }
}
