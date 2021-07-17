import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { GetDepartmentsService } from '../services/departments.service';

@Component({
  selector: 'app-choose-department',
  templateUrl: './choose-department.component.html',
  styleUrls: ['./choose-department.component.css']
})
export class ChooseDepartmentComponent implements OnInit {

  departments:any = {}

  constructor(private getDepartmentService:GetDepartmentsService,
              private activateRoute:ActivatedRoute) { }

  chooseDepartmentForm = new FormGroup({
    department : new FormControl('', [Validators.required])
  })

  get departmentGet(){
    return this.chooseDepartmentForm.get("department")
  }



  ngOnInit(): void {
    this.getDepartmentService.getDepartmentsData().subscribe(
      data =>{
        this.departments = data;
        console.log(this.departments);
      }
    )
  }

  submitDepartments(){

  }

}
