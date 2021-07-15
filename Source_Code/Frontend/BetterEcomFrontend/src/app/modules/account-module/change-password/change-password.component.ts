import { Component, OnInit } from '@angular/core';
import { GetProfileDataService } from '../services/get-profile-data.service';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators } from '@angular/forms';
import {HttpClient} from '@angular/common/http'
import { SaveProfileChangesService } from '../services/save-profile-changes.service';
import { passwordValidator } from 'src/app/validators/password-confirm-validator';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {

  password:string
  misMatch: boolean = false

  constructor(private router: Router,
              private saveProfileService : SaveProfileChangesService) {}

    changePasswordForm = new FormGroup({
      oldPassword: new FormControl('', [Validators.required]),
      newPassword: new FormControl('', [Validators.required]),
      confirmPassword: new FormControl('', [Validators.required])
    }, [passwordValidator]);

    get oldPasswordGet() {
      return this.changePasswordForm.get('oldPassword')
    }

    get newPasswordGet() {
      return this.changePasswordForm.get('newPassword')
    }

    get confirmPasswordGet() {
      return this.changePasswordForm.get('confirmPassword')
    }


  ngOnInit(): void {
    }



  submitNewPass(oldPassword, newPassword, confirmPassword) {



    this.saveProfileService.changePassword(oldPassword, newPassword).subscribe(

      response => {
        alert("password changed sucessfuly")
        this.router.navigate(['/profile/' + localStorage.getItem("type")])
      },
      error =>
      {
        alert("old password is wrong failed to change")
      }

    )


  }


}

// <p *ngIf="ConfirmPassword.value != NewPassword.value && confirmPasswordGet.touched" style="color: red;">confirm new password has to match with new password</p>
