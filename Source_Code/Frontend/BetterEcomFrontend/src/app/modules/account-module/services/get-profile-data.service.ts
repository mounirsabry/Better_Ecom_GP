import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GetProfileDataService {

  constructor(private httClient:HttpClient) { }

  getProfileData(type:string){

    console.log("https://localhost:44361/profile/"+ type + "/" + localStorage.getItem("ID"))

    return this.httClient.get<any>("https://localhost:44361/profile/"+ type + "/" + localStorage.getItem("ID"))
  }
}
