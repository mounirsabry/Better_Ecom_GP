import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router'
import { UpdateDataAdminService } from '../services/update-data-admin.service';

@Component({
  selector: 'app-create-account',
  templateUrl: './create-account.component.html',
  styleUrls: ['./create-account.component.css']
})
export class CreateAccountComponent implements OnInit {

  constructor(private activatedRoute:ActivatedRoute,
              private updateDataService:UpdateDataAdminService,
              private router : Router) { }

  createAccountForm = new FormGroup({
    id: new FormControl('', [Validators.required]),
    nationalID: new FormControl('', [Validators.required])
  })

  get idGet() {
    return this.createAccountForm.get("id")
  }

  get nationalIDGet() {
    return this.createAccountForm.get("nationalID")
  }

  type:string
  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params: ParamMap) => {
      this.type = params.get('type')
    })
  }

  submitAccount()
  {
    this.updateDataService.createAccount(this.createAccountForm).subscribe(
      response => {
        alert("Account created successfully")
        this.router.navigate(['/adminHomePage/'])
      },
      error => {
        alert("ID already exits or no match was found for the national id")
      }
    )
  }

}
