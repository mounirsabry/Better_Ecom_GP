import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DepartmentsService } from '../services/departments.service';

@Component({
  selector: 'app-change-course-department',
  templateUrl: './change-course-department.component.html',
  styleUrls: ['./change-course-department.component.css']
})
export class ChangeCourseDepartmentComponent implements OnInit {

  constructor(private departmentService : DepartmentsService) { }

  view_department_list : Array<string> = []

  changeCourseDepartmentForm = new FormGroup({
    CourseID : new FormControl('', [Validators.required]),
    NewDepartmentCode : new FormControl('', [Validators.required])
  })

  get courseIdGet(){
    return this.changeCourseDepartmentForm.get('CourseID');
  }

  get departmentCodeGet(){
    return this.changeCourseDepartmentForm.get('NewDepartmentCode');
  }

  ngOnInit(): void {
    this.departmentService.getDepartmentsData().subscribe(
      data =>{
        var department_names = data.map(function(obj) {return obj.department_name});
        this.view_department_list = department_names;
        //var department_code = data.map(function(obj) {return obj.department_code});
        //this.department_code_list = department_code;
      }
    )
  }

  submitCourseToDepartment(){

  }

}
