import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CourseInstanceService } from '../services/course-instance.service';

@Component({
  selector: 'app-student-grade',
  templateUrl: './student-grade.component.html',
  styleUrls: ['./student-grade.component.css']
})
export class StudentGradeComponent implements OnInit {

  constructor(private route : ActivatedRoute,
    private courseInstanceService : CourseInstanceService) { }

  instance_id : number
  student_id : number
  grade : string

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      console.log(params);
      this.instance_id = +params.instanceID;
      console.log(this.instance_id);
    })
    this.student_id = +localStorage.getItem('ID');
    this.getStudentGrade();
  }

  getStudentGrade(){
    this.courseInstanceService.getStudentCourseInstanceGrade(this.student_id, this.instance_id).subscribe(
      data =>{
        console.log(data);
        this.grade = data.result;
      },
      error =>{
        console.log(error.error);
      }
    )
  }

  getStudentGpa(){
      
  }

}
