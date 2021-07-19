import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { DepartmentCoursesService } from '../services/department-courses.service';
import { DepartmentsService } from '../services/departments.service';

@Component({
  selector: 'app-view-departments-courses',
  templateUrl: './view-departments-courses.component.html',
  styleUrls: ['./view-departments-courses.component.css']
})
export class ViewDepartmentsCoursesComponent implements OnInit {

  //department_name_list:Array<String> = []
  type:string
  courses:Array<any> = []// array of courses obj.
  department_code_list:Array<any> = []

  constructor(private activatedRoute:ActivatedRoute,
              private departmentCoursesService:DepartmentCoursesService,
              private departmentService:DepartmentsService) { }

  viewDepartmentCoursesForm = new FormGroup({
    DepartmentCode : new FormControl('', [Validators.required])
  })

  get departmentGet(){
    return this.viewDepartmentCoursesForm.get("DepartmentCode");
  }


  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params: ParamMap) => {
      this.type = params.get('type')
    })
    this.departmentService.getDepartmentsData().subscribe(
      data =>{
        //var department_names = data.map(function(obj) {return obj.department_name});
        //this.department_name_list = department_names;

        var department_code = data.map(function(obj) {return obj.department_code});
        this.department_code_list = department_code;
      }
    )
  }

  viewCourses(){
    this.departmentCoursesService.getDepartmentCourses(this.departmentGet.value).subscribe(
      data =>{
        this.courses = data;
        /*for(const [key, value] of Object.entries(data)) {
          //console.log(key, value);
          this.courses.push(value);
        }*/
        console.log(Object.values(data));
        for(let value of Object.values(data)){
          //console.log(value);
          //this.courses.push(value);
          for(let valuee of Object.values(value)){
            console.log(valuee);
          }
        }
        //console.log(this.courses);
      }
    )
  }

  objectValues(obj){
    return Object.values(obj);
  }

  objectKeys(obj){
    return Object.keys(obj);
  }

}
