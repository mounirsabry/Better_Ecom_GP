import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SubmitDepartmentPriorityListService {

  constructor(private httpClient:HttpClient) { }

  submitDepartmentPriorityList(department_list : Array<any>){
    var stringID = localStorage.getItem('ID');
    var numID : number = +stringID; // changes the type of the ID from string to integer

    var newDep_list = [numID].concat(department_list);
    var department_code = ['StudentID','DepartmentCode1', 'DepartmentCode2', 'DepartmentCode3', 'DepartmentCode4', 'DepartmentCode5'];
    var result = newDep_list.reduce(function(result, field, index){
      result[department_code[index]] = field;
      return result;
    }, {})
    var temp = {}
    var finalResult = [];
    
    console.log(result);
    return this.httpClient.post<any>('https://localhost:44361/department/ChooseDepartments', result);
  }

  handleError(error : HttpErrorResponse){
    return throwError(error);
  }
}

