import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { DepartmentsService } from '../../department-module/services/departments.service';
import { RegisterNewStudentOrInstructorService } from '../services/register-new-student-or-instructor.service';

@Component({
  selector: 'app-register-new-student-or-instructor',
  templateUrl: './register-new-student-or-instructor.component.html',
  styleUrls: ['./register-new-student-or-instructor.component.css']
})
export class RegisterNewStudentOrInstructorComponent implements OnInit {

  departmentCodes = []
  constructor(private activatedRoute:ActivatedRoute,
              private regNewStdntOrIns : RegisterNewStudentOrInstructorService,
              private departmentsService : DepartmentsService) { }

  type:string
  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params: ParamMap) => {
      this.type = params.get('type')
    });

    if(this.type == 'student'){

      this.registerForm.addControl('High_school_type',new FormControl('',Validators.required))
      this.registerForm.addControl('Entrance_year',new FormControl('',Validators.required))
      this.registerForm.addControl('Department_code', new FormControl('GE'))

    }else if(this.type = 'instructor'){


      this.registerForm.addControl('University',new FormControl('',Validators.required))
      this.registerForm.addControl('Graduation_year',new FormControl('',Validators.required))

      this.registerForm.addControl('Department_code', new FormControl('',Validators.required))

      this.departmentsService.getDepartmentsData().subscribe(
        data =>{

          this.departmentCodes = data.map(function(obj) {return obj.department_code});
        }
      )

    }


  }

  registerForm = new FormGroup({

    Full_name : new FormControl('',Validators.required),
    Address : new FormControl('',Validators.required),
    Nationality : new FormControl('',Validators.required),
    National_id : new FormControl('',Validators.required),
    Birth_date : new FormControl('',Validators.required),
    Gender: new FormControl('',Validators.required),
  }
  )

  get fullNameGet() {
    return this.registerForm.get('Full_name')
  }

  get departmentCodeGet() {
    return this.registerForm.get('Department_code')
  }

  get addressGet() {
    return this.registerForm.get('Address')
  }

  get nationalityGet() {
    return this.registerForm.get('Nationality')
  }

  get nationalIDGet() {
    return this.registerForm.get('National_id')
  }

  get birthDateGet() {
    return this.registerForm.get('Birth_date')
  }

  get highSchoolTypeGet() {
    return this.registerForm.get('High_school_type')
  }

  get entranceYearGet() {
    return this.registerForm.get('Entrance_year')
  }

  get universityGet() {
    return this.registerForm.get('University')
  }

  get graduationYearGet() {
    return this.registerForm.get('Graduation_year')
  }

  get genderGet() {
    return this.registerForm.get('Gender')
  }



  register(){
    this.registerForm['National_id'] = this.registerForm['national_id'] +''
    this.regNewStdntOrIns.register(this.type,this.registerForm.value).subscribe(
      response =>{
        alert('registeration successfull!')
      },
      error =>{
        alert('registeration failed! ' + this.type + ' maybe already registered or birth date is unrealistic')
      }
    )

  }
}
