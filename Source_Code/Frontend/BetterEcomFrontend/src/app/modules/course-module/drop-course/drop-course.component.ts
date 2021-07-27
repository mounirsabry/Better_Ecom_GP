import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DropCourseServiceService } from '../services/drop-course-service.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-drop-course',
  templateUrl: './drop-course.component.html',
  styleUrls: ['./drop-course.component.css']
})
export class DropCourseComponent implements OnInit {

  constructor(private dropCourseService : DropCourseServiceService,
    private route : ActivatedRoute) { }

  adminDropForm = new FormGroup ({
    selectType : new FormControl('', Validators.required),
    user_id : new FormControl('', Validators.required),
    course_instance_id : new FormControl('', Validators.required)
  })

  get type_get(){
    return this.adminDropForm.get('selectType');
  }

  get user_id_get(){
    return this.adminDropForm.get('user_id');
  }

  get course_instance_id_get(){
    return this.adminDropForm.get('course_instance_id');
  }

  studentDropForm = new FormGroup ({
    student_id : new FormControl('', Validators.required),
    course_instance_id : new FormControl('', Validators.required)
  })

  get student_id_get(){
    return this.studentDropForm.get('student_id');
  }

  get student_course_id(){
    return this.studentDropForm.get('course_instance_id');
  }

  instructorDropForm = new FormGroup ({
    instructor_id : new FormControl('', Validators.required),
    course_instance_id : new FormControl('', Validators.required)
  })

  get instructor_id_get(){
    return this.instructorDropForm.get('instructor_id');
  }

  get instructor_course_id(){
    return this.instructorDropForm.get('course_instance_id');
  }

  user_type : string
  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.user_type = params.type;
      console.log(this.user_type);
    })
  }

  isAdmin(){
    if(this.user_type === 'admin'){return true;}
    else {return false;}
  }

  isStudent(){
    if(this.user_type === 'student'){return true;}
    else {return false;}
  }

  isInstructor(){
    if(this.user_type === 'instructor'){return true;}
    else {return false;}
  }

  dropUser(){
    var type = this.type_get.value;
    console.log(type);

    var user_id = this.user_id_get.value;
    var course_instance_ID = this.course_instance_id_get.value;

    console.log(user_id);
    console.log(course_instance_ID);

    if(type === 'instructor'){
      this.dropCourseService.dropInstructorFromCourseInstance(course_instance_ID, user_id).subscribe(
        response =>{
          console.log(response);
          alert("Course dropped for Instructor (if user is not registered in course nothing will happen)");

        },
        error =>{
          console.log(error.error);
          alert("failed, course maybe be closed");
        }
      )
    }
    else if(type === 'student'){
      this.dropCourseService.dropStudentFromCourseInstance(course_instance_ID, user_id).subscribe(
        response =>{
          console.log(response);
          alert("Course dropped for Student (if user is not registered in course nothing will happen)");

        },
        error =>{
          console.log(error.error);
          alert("failed,course maybe be closed");
        }
      )
    }
  }
}
