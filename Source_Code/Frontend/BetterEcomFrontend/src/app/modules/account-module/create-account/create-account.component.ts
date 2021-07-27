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
    id: new FormControl('', [Validators.required])
  })

  get idGet() {
    return this.createAccountForm.get("id")
  }

  type:string
  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params: ParamMap) => {
      this.type = params.get('type')
    })
  }

  submitAccount()
  {
    this.updateDataService.createAccount(this.idGet.value,this.type).subscribe(
      response => {
        alert("Account created successfully")
       // this.router.navigate(['/adminHomePage/'])
      },
      error => {
        alert("no matching id was found or account already created")
      }
    )
  }

}
