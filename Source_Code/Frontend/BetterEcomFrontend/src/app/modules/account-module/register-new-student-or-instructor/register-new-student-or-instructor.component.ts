import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-register-new-student-or-instructor',
  templateUrl: './register-new-student-or-instructor.component.html',
  styleUrls: ['./register-new-student-or-instructor.component.css']
})
export class RegisterNewStudentOrInstructorComponent implements OnInit {

  constructor(private activatedRoute:ActivatedRoute,
              private registerStdntOrInsService:RegisterNewStudentOrInstructorComponent) { }

  type:string
  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params: ParamMap) => {
      this.type = params.get('type')
    })
  }


  register(){

  }
}
