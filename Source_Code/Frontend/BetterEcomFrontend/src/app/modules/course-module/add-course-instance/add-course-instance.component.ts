import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CourseInstanceService } from '../services/course-instance.service';

@Component({
  selector: 'app-add-course-instance',
  templateUrl: './add-course-instance.component.html',
  styleUrls: ['./add-course-instance.component.css']
})
export class AddCourseInstanceComponent implements OnInit {

  constructor(private courseInstanceService: CourseInstanceService) { }

  coursTerm = ['First','Second','Summer','Other']
  addCourseInstanceForm = new FormGroup({
    UserID: new FormControl('',),
    //Instance_id : new FormControl('', Validators.required),
    Course_code: new FormControl('', Validators.required),
    Course_year: new FormControl(''),
    Course_term: new FormControl('', Validators.required),
    Credit_hours: new FormControl('', Validators.required)
  })

  get instance_id_get() {
    return this.addCourseInstanceForm.get('Instance_id');
  }

  get course_code_get() {
    return this.addCourseInstanceForm.get('Course_code');
  }

  get course_year_get() {
    return this.addCourseInstanceForm.get('Course_year');
  }

  get course_term_get() {
    return this.addCourseInstanceForm.get('Course_term');
  }

  get credit_hours_get() {
    return this.addCourseInstanceForm.get('Credit_hours');
  }



  ngOnInit(): void {
    this.addCourseInstanceForm.controls['UserID'].setValue(+localStorage.getItem('ID'));
  }

  addCourseInstance() {
    console.log(this.addCourseInstanceForm.value['Course_term'])
    console.log(this.coursTerm.indexOf(this.addCourseInstanceForm.value['Course_term']))
    this.addCourseInstanceForm.value['Course_term'] = this.coursTerm.indexOf(this.addCourseInstanceForm.value['Course_term'])
    this.courseInstanceService.addCourseInstance(this.addCourseInstanceForm.value).subscribe(
      data => {
        alert("Course Instance added");
      },
      error => {
        console.log(error.error);
        alert("failed");
      }
    )
  }
}
