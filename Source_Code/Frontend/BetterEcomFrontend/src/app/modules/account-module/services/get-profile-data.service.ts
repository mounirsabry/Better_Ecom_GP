import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GetProfileDataService {

  constructor(private httClient:HttpClient) { }

  getProfileData(type:string){


    return this.httClient.get<any>("https://localhost:44361/profile/GetProfile" )
  }
}
