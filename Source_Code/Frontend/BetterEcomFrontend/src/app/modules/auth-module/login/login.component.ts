import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validator, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap, Router } from '@angular/router'
import { AuthService } from '../services/auth.service';

// admin: 11, instructor:31, student:20210001
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  type: string
  errorMessage: string = ""


  loginForm = new FormGroup({
    ID: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required])
  })


  get IDget() {
    return this.loginForm.get('ID')
  }

  get passwordGet() {
    return this.loginForm.get('password')
  }

  constructor(
    private activatedRoute: ActivatedRoute,
    private authService: AuthService,
    private router: Router,
    private httpClient: HttpClient
  ) { }

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params: ParamMap) => {
      this.type = params.get('type')
      this.errorMessage = params.get('errorMessage')
    })
  }

  login(ID, password) {


    this.authService.login(parseInt(ID), password, this.type).subscribe(
      // token => this.token = token,
      response => {
        localStorage.setItem('token', response.token);
        if (this.type == "student") {
          this.router.navigate(['studentHomePage'])
        } else if (this.type == "instructor") {
          this.router.navigate(['instructorHomePage'])
        } else if (this.type == "admin") {
          this.router.navigate(['adminHomePage'])
        }
      },
      error => {
        if (error.status == 401) {
          // navigate to your self.
          this.router.navigate(['./', { errorMessage: "you entered either wrong id or password" }], { relativeTo: this.activatedRoute })
        }
      }
    )




  }

}
