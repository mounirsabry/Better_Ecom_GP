import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap } from '@angular/router';
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

  /*courses = [
    {'natural language processing' : {'prequisteCourses' : ['machine learning', 'algorithm', 'data structure']}}
  ]*/

  constructor(private departmentService : DepartmentsService,
              private activateRoute:ActivatedRoute) { }

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
    this.departmentService.getStudentPriorityList().subscribe(
      priority_list =>{
        console.log(priority_list);
      }
    )

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
      },
      (error) =>{
        console.log(error.error);
      }
    )
  }

}
