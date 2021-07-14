import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ViewProfileComponent } from './view-profile/view-profile.component';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SaveProfileChangesService } from './services/save-profile-changes.service';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { CreateAccountComponent } from './create-account/create-account.component';
import { AdminResetPasswordComponent } from './admin-reset-password/admin-reset-password.component';
import { RegisterationsAndAccountsManagementComponent } from './registerations-and-accounts-management/registerations-and-accounts-management.component';
import { RegisterNewStudentOrInstructorService } from './services/register-new-student-or-instructor.service';
import { RegisterNewStudentOrInstructorComponent } from './register-new-student-or-instructor/register-new-student-or-instructor.component';



@NgModule({
  declarations: [
    ViewProfileComponent,
     ChangePasswordComponent,
      RegisterationsAndAccountsManagementComponent,
      RegisterNewStudentOrInstructorComponent,
      CreateAccountComponent,
      AdminResetPasswordComponent



    ],
  imports: [
    CommonModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule
  ],
  exports:[
    ViewProfileComponent,
    RegisterationsAndAccountsManagementComponent
  ],
  providers:[SaveProfileChangesService,RegisterNewStudentOrInstructorService]
})
export class AccountModuleModule { }
