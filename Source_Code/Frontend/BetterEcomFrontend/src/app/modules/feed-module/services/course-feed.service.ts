import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CourseFeedService {

  constructor(private httpClient: HttpClient) { }

  getInstanceFeed(courseInstanceID:number){
    return this.httpClient.get("https://localhost:44361/Feed/GetCourseInstanceFeed/" + courseInstanceID)
  }

  addToFeed(obj:any){

    return this.httpClient.post("https://localhost:44361/Feed/AddToCourseInstanceFeed",obj)

  }

  delete(instanceID:number,feedID:number){
    return this.httpClient.delete("https://localhost:44361/Feed/DeleteFromCourseInstanceFeed/"+instanceID+'/'+feedID)
  }
}
