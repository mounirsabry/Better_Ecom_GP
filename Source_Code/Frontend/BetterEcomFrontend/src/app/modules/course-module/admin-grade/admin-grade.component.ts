import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CourseInstanceService } from '../services/course-instance.service';

@Component({
  selector: 'app-admin-grade',
  templateUrl: './admin-grade.component.html',
  styleUrls: ['./admin-grade.component.css']
})
export class AdminGradeComponent implements OnInit {

  constructor(private route : ActivatedRoute,
    private courseInstanceService : CourseInstanceService) { }

  ngOnInit(): void {
  }

}
