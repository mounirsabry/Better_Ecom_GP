import { ThisReceiver } from '@angular/compiler';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { element } from 'protractor';
import { DepartmentsService } from '../services/departments.service';

@Component({
  selector: 'app-choose-department',
  templateUrl: './choose-department.component.html',
  styleUrls: ['./choose-department.component.css']
})
export class ChooseDepartmentComponent implements OnInit {

  departmentList:Array<String> = []
  departmentSelected:Array<any> = []

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
    var array;
    this.departmentService.getDepartmentsData().subscribe(
      data =>{
        var department_names = data.map(function(obj) {return obj.department_name});
        this.departmentList = department_names;
        this.departmentList.splice(3,1); // removes the general department
      }
    )
  }

  addDepartment(){
    this.departmentSelected.push(this.departmentGet.value)
    for(let i of this.departmentSelected){
      this.departmentList.forEach((element, index)=>{
        if(i === element) this.departmentList.splice(index, 1);
      });
    }
  }

  removeDepartment(){
    this.departmentList.push(this.remDepartmentGet.value)
    for(let i of this.departmentList){
      this.departmentSelected.forEach((element, index)=>{
        if(i === element) this.departmentSelected.splice(index, 1);
      })
    }
  }

  hasError(){
    if(this.departmentSelected.length < 5){
      return true;
    }
    else{
      return false;
    }
  }

  submitDepartments(){
    this.departmentService.submitDepartmentPriorityList(this.departmentSelected).subscribe(
      (response) =>{
        console.log(response);
      },
      (error) =>{
        console.log(error.error);
      }
    )
  }

}
