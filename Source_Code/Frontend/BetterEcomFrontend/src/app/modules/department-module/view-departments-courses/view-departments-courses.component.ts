import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { DepartmentCoursesService } from '../services/department-courses.service';

@Component({
  selector: 'app-view-departments-courses',
  templateUrl: './view-departments-courses.component.html',
  styleUrls: ['./view-departments-courses.component.css']
})
export class ViewDepartmentsCoursesComponent implements OnInit {

  constructor(private activatedRoute:ActivatedRoute,
              private departmentCoursesService:DepartmentCoursesService) { }

  type:string
  courses = []// array of courses obj.
  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params: ParamMap) => {
      this.type = params.get('type')
    })
  }

}
