import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-course-instance-page',
  templateUrl: './course-instance-page.component.html',
  styleUrls: ['./course-instance-page.component.css']
})
export class CourseInstancePageComponent implements OnInit {

  logedInType = localStorage.getItem('type')
  constructor(private activatedRoute:ActivatedRoute) { }

  instanceID:string
  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params: ParamMap) => {
      this.instanceID = params.get('instanceID')
    })
  }

}
