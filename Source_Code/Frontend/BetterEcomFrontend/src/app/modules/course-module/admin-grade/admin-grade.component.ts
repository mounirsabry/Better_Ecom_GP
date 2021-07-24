import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CourseInstanceService } from '../services/course-instance.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { concat } from 'rxjs';

@Component({
  selector: 'app-admin-grade',
  templateUrl: './admin-grade.component.html',
  styleUrls: ['./admin-grade.component.css']
})


export class AdminGradeComponent implements OnInit {

  constructor(private route : ActivatedRoute,
    private courseInstanceService : CourseInstanceService) { }

    setGradeForm = new FormGroup({
      StudentID : new FormControl('', Validators.required),
      CourseInstanceID : new FormControl('', Validators.required),
      Grade : new FormControl('', Validators.required)
    })

    get student_id_get(){
      return this.setGradeForm.get('StudentID');
    }

    get course_intance_id_get(){
      return this.setGradeForm.get('CourseInstanceID');
    }

    get grade_get(){
      return this.setGradeForm.get('Grade');
    }

    studentGpaForm = new FormControl({
      StudentID : new FormControl('', Validators.required)
    })

    studentCourseInstanceGrades : Array<string> = ['APlus',
      'A',
      'BPlus',
      'B',
      'CPlus',
      'C',
      'DPlus',
      'D',
      'F']

  ngOnInit(): void {
  }

  setGrade(){
    var temp = this.studentCourseInstanceGrades.findIndex(x => x === this.grade_get.value);
    this.setGradeForm.controls['Grade'].setValue(temp);
    console.log(this.setGradeForm.value);
    this.courseInstanceService.setStudentCourseInstanceGrade(this.setGradeForm.value).subscribe(
      respone =>{
        alert(respone)
      },
      error =>{
        alert(error.error);
      }
    )
  }

  getStudentGpa(){

  }

}
