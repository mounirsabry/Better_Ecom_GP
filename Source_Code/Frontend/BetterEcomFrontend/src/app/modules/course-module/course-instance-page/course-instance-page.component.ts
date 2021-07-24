import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-course-instance-page',
  templateUrl: './course-instance-page.component.html',
  styleUrls: ['./course-instance-page.component.css']
})
export class CourseInstancePageComponent implements OnInit {

  constructor(private route : ActivatedRoute) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      console.log(params);
    })
  }

}
