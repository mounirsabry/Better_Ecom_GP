import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GeneralFeedService {

  constructor(private httpClient:HttpClient) { }

  addToGeneralFeed(content:any){

    return this.httpClient.post("https://localhost:44361/Feed/AddToGeneralFeeds",content)
  }

  getGeneralFeed(){
    return this.httpClient.get<any>("https://localhost:44361/Feed/GetGeneralFeeds")
  }

  delete(ID:number){
    return this.httpClient.delete("https://localhost:44361/Feed/DeleteFromGeneralFeeds/"+ID)
  }


}
