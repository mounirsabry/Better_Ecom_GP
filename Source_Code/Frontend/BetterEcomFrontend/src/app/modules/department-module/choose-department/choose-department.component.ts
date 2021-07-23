import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DepartmentsService } from '../services/departments.service';

@Component({
  selector: 'app-choose-department',
  templateUrl: './choose-department.component.html',
  styleUrls: ['./choose-department.component.css']
})
export class ChooseDepartmentComponent implements OnInit {

  department_name_list:Array<String> = []
  department_selected_name:Array<any> = []
  department_code_list:Array<any> = []
  department_selected_code:Array<any> = []
  department_priority_list:Array<any> = []

  /*courses = [
    {'natural language processing' : {'prequisteCourses' : ['machine learning', 'algorithm', 'data structure']}}
  ]*/

  constructor(private departmentService : DepartmentsService) { }

  chooseDepartmentForm = new FormGroup({
    department : new FormControl('', [Validators.required])
  })

  removeDepartmentForm = new FormGroup({
    remDepartment :new FormControl('', [Validators.required]),
    submitList : new FormControl('')
  })

  get departmentGet(){
    return this.chooseDepartmentForm.get("department");
  }

  get remDepartmentGet(){
    return this.removeDepartmentForm.get("remDepartment");
  }

  get submitedListGet(){
    return this.removeDepartmentForm.get("submitList");
  }

  ngOnInit(): void {
    var stringID = localStorage.getItem('ID');
    var numID : number = +stringID; // changes the type of the ID from string to integer
    this.departmentService.getStudentPriorityList(numID).subscribe(
      priority_list =>{
        console.log(priority_list);

        if(priority_list.length < 1){

          this.departmentService.getDepartmentsData().subscribe(
            data =>{
              var department_names = data.map(function(obj) {return obj.department_name});
              this.department_name_list = department_names;
              this.department_name_list.splice(3,1); // removes the general department
              var department_code = data.map(function(obj) {return obj.department_code});
              this.department_code_list = department_code;
              this.department_code_list.splice(3, 1);
            }
          )
        }
        else{
          var list = priority_list.sort(function(a, b){
            return a.priority - b.priority;
          })
          this.department_priority_list = list.map(function(obj) {return obj.department_code});
          this.mapCodeToName();
        }
      },
      error =>{
        console.log(error.error);
      }
    )
  }

  addDepartment(){
      this.department_selected_name.push(this.departmentGet.value);
      for(let i of this.department_selected_name){
        this.department_name_list.forEach((element, index)=>{
          if(i === element){
            this.department_name_list.splice(index, 1);
            this.department_selected_code.push(this.department_code_list[index])
            this.department_code_list.splice(index, 1);
          }
        });
      }
      console.log(this.department_code_list);
      console.log(this.department_selected_code);
      this.chooseDepartmentForm.controls.department.reset();
  }

  removeDepartment(){
      this.department_name_list.push(this.remDepartmentGet.value)
      for(let i of this.department_name_list){
        this.department_selected_name.forEach((element, index)=>{
          if(i === element){
            this.department_selected_name.splice(index, 1);
            this.department_code_list.push(this.department_selected_code[index]);
            this.department_selected_code.splice(index, 1);
          }
        })
      }
      this.removeDepartmentForm.controls.remDepartment.reset();
  }

  hasError(){
    if(this.department_selected_name.length < 5){
      return true;
    }
    else{
      return false;
    }
  }

  submitDepartments(){
    this.departmentService.submitDepartmentPriorityList(this.department_selected_code).subscribe(
      (response) =>{
        console.log(response);
        alert("Department List Submited");
      },
      (error) =>{
        console.log(error.error);
        alert("failed");
      }
    )
  }

  mapCodeToName(){
    for(let dep of this.department_priority_list){
      if(dep === 'CS') {
        this.department_selected_name.push('Computer Science');
        this.department_selected_code.push('CS');
      }
      else if(dep === 'IT') {
        this.department_selected_name.push('Information Technology');
        this.department_selected_code.push('IT');
      }
      else if(dep === 'AI') {
        this.department_selected_name.push('Artifical Intelligence');
        this.department_selected_code.push('AI');
      }
      else if(dep === 'IS') {
        this.department_selected_name.push('Information Systems');
        this.department_selected_code.push('IS');
      }
      else if(dep === 'DS') {
        this.department_selected_name.push('Decision Support');
        this.department_selected_code.push('DS');
      }
    }
  }

  isEmpty(list : Array<any>){
    if(list.length > 0) {return false;}
    else {return true;}
  }
}
