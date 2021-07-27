import { ChangeDetectionStrategy } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { GetProfileDataService } from '../services/get-profile-data.service';
import { SaveProfileChangesService } from '../services/save-profile-changes.service';
import { CourseInstanceService } from '../../course-module/services/course-instance.service';

//"let property of user | keyvalue
@Component({
  selector: 'app-view-profile',
  templateUrl: './view-profile.component.html',
  styleUrls: ['./view-profile.component.css']
})
export class ViewProfileComponent implements OnInit {

  user:any = {

  }
  id:string



  profileForm = new FormGroup({
  })


  constructor(
    private getProfileDataService:GetProfileDataService,
    private activatedRoute:ActivatedRoute,
    private saveProfileChangesService:SaveProfileChangesService,
    private courseInstanceService:CourseInstanceService
    ) { }

    student_gpa : number
    hasGpa : boolean
  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params:ParamMap)=>{

      this.id = localStorage.getItem('ID')
      this.getProfileDataService.getProfileData(params.get('type')).subscribe(
        data =>{
          this.user = data;
          // putting the for outside will make it run before user is populated.
          for(let key of Object.keys(this.user)){
            //as setting disable in the html doesn't work with reactive forms.
            // nullvalidator is a validator that does nothing.
           let formControl = new FormControl({value:this.user[key],disabled:this.disable(key)})
           formControl.setValidators([(key == "email"? Validators.email : Validators.nullValidator)])
            this.profileForm.addControl(key,formControl)
          }

        }
      )

    })

  }


  inputType(key){
      if(key == "mobile_number" || key == "phone_number"){
        return "text"
      }else{
        return "text"
      }
  }

  // ask the other bros about this.
  disable(name:string){
    return !(name == 'additional_info' || name == 'address' ||
    name == 'email' || name == 'mobile_number' || name == 'phone_number' || name == 'contact_info')
  }

  get emailGet(){
    return this.profileForm.get('email')
  }

  changeColor(name){
    return (this.profileForm.get(name).dirty? 'red' : 'black')
  }

  saveChanges(){


    // convert first letters to capital
    let obj = {}
    for(let key of Object.keys(this.profileForm.value)){
      let captitalKey = key.charAt(0).toUpperCase() + key.slice(1);

      if(this.profileForm.value[key] == null){
        obj[captitalKey] = ""

      }else{
        obj[captitalKey] = this.profileForm.value[key]
      }
    }

   // console.log(this.user)
    this.saveProfileChangesService.saveChanges(obj).subscribe(
      responnse =>{
        alert("changes saved successfully!")
      },
      error =>{
        alert("failed to save changes!")
      }
    )
  }

  getStudentGpa(){

    this.courseInstanceService.getStudentGPA(+this.id).subscribe(
      respone =>{
        console.log(respone);
        this.student_gpa = respone.gpa;
        if (this.student_gpa != null) {this.hasGpa = true;}
        else {this.hasGpa = false;}
      },
      error =>{
        alert(error.error);
      }
    )

  }




}
