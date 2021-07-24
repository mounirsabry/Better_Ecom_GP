import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DepartmentsService } from '../services/departments.service';

// this component admin set department for student

@Component({
  selector: 'app-admin-department',
  templateUrl: './admin-department.component.html',
  styleUrls: ['./admin-department.component.css']
})
export class AdminDepartmentComponent implements OnInit {

  view_priority_list: Array<string> = []

  constructor(private departmentService: DepartmentsService) { }

  setDepartmentForm = new FormGroup({
    StudentID: new FormControl('', [Validators.required]),
    DepartmentCode: new FormControl('', [Validators.required])
  })

  get studentIdGet() {
    return this.setDepartmentForm.get('StudentID');
  }

  get departmentCodeGet() {
    return this.setDepartmentForm.get('DepartmentCode');
  }

  ngOnInit(): void {
  }

  getStudentList() {
    this.departmentService.getStudentPriorityList(this.studentIdGet.value).subscribe(
      priority_list =>{
        if(priority_list.length > 0){
          console.log(priority_list);
          var list = priority_list.sort(function(a, b){
            return a.priority - b.priority;
          })
          this.view_priority_list = list.map(function(obj) {return obj.department_code});
        }
        else {alert("Student has not submited any priority list");}
      }
    )
  }

  setDepartment() {
    this.departmentService.setDepartmentForStudent(this.setDepartmentForm.value).subscribe(
      response => {
        alert("Department has been set");
      },
      error => {
        console.log(error.error);
        alert("failed");
      }
    )
  }

}
