import { Component, OnInit } from '@angular/core';
import { LateRegisterationService } from '../../late-registeration-module/services/late-registeration.service';

@Component({
  selector: 'app-admin-course-page',
  templateUrl: './admin-course-page.component.html',
  styleUrls: ['./admin-course-page.component.css']
})
export class AdminCoursePageComponent implements OnInit {

  constructor(private lateRegisterationServic : LateRegisterationService) { }

  ngOnInit(): void {
  }

}
