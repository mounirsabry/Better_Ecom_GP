import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-course-instance-page',
  templateUrl: './course-instance-page.component.html',
  styleUrls: ['./course-instance-page.component.css']
})
export class CourseInstancePageComponent implements OnInit {

  constructor(private route : ActivatedRoute) { }
  instance_id : number
  ngOnInit(): void {
    this.route.params.subscribe(params => {
      console.log(params);
      this.instance_id = params.instanceID;
      console.log(this.instance_id);
    })
  }

}
