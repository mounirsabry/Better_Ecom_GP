import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router'
import { UpdateDataAdminService } from '../services/update-data-admin.service';

@Component({
  selector: 'app-admin-reset-password',
  templateUrl: './admin-reset-password.component.html',
  styleUrls: ['./admin-reset-password.component.css']
})
export class AdminResetPasswordComponent implements OnInit {

  /*public types = [
    {"type": "Student"},
    {"type": "Instructor"}
  ];*/

  constructor(private activatedRoute:ActivatedRoute,
              private updateDataService:UpdateDataAdminService,
              private router : Router) { }

  resetPasswordForm = new FormGroup({
    type : new FormControl('', [Validators.required]),
    id: new FormControl('', [Validators.required]),
    nationalID: new FormControl('', [Validators.required])
  })

  get typeGet() {
    return this.resetPasswordForm.get("type")
  }

  get idGet() {
    return this.resetPasswordForm.get("id")
  }

  get nationalIDGet() {
    return this.resetPasswordForm.get("nationalID")
  }

  ngOnInit(): void {
  }

  submit(){
    this.updateDataService.resetPassword(this.resetPasswordForm).subscribe(
      response => {
        alert("Password has been reset to the user national id")
        this.router.navigate(['/adminHomePage/'])
      },
      error => {
        alert("ID doesn't exit or no match was found for the national id")
      }
    )
  }

}

