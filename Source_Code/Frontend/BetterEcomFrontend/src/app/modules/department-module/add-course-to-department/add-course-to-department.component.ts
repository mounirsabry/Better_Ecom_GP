import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-add-course-to-department',
  templateUrl: './add-course-to-department.component.html',
  styleUrls: ['./add-course-to-department.component.css']
})
export class AddCourseToDepartmentComponent implements OnInit {

  constructor(private activatedRoute:ActivatedRoute) { }

  departmentCode:string
  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params: ParamMap) => {
      this.departmentCode = params.get('departmentCode')
    })
  }

}
