import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminHomePageComponent } from './components/admin-home-page/admin-home-page.component';
import { InstructorHomePageComponent } from './components/instructor-home-page/instructor-home-page.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { StartPageComponent } from './components/start-page/start-page.component';
import { StudentHomePageComponent } from './components/student-home-page/student-home-page.component';
import { AdminResetPasswordComponent } from './modules/account-module/admin-reset-password/admin-reset-password.component';
import { ChangePasswordComponent } from './modules/account-module/change-password/change-password.component';
import { CreateAccountComponent } from './modules/account-module/create-account/create-account.component';
import { ViewProfileComponent } from './modules/account-module/view-profile/view-profile.component';
import { LoginComponent } from './modules/auth-module/login/login.component';

const routes: Routes = [
  // i did redirecto to startPage instead of normal cuz you can't pass optional paramters to '' url.
  {path:'', redirectTo:'startPage',pathMatch:'full'},
  {path:'startPage',component:StartPageComponent},
  {path:'login/:type',component:LoginComponent},
  // this is the url that will be give to the acual admin as if he mistyped it he will be redirected
  // to page-not-found component, but if the admin mistyped login/admin (e.g login/admn)
  // he won't be redirected to page not found.
  {path:'adminLogin',redirectTo:'login/admin',pathMatch:'full'},
  {path:'studentHomePage',component:StudentHomePageComponent},
  {path:'instructorHomePage',component:InstructorHomePageComponent},
  {path:'adminHomePage',component:AdminHomePageComponent},
  {path:'profile/:type', component:ViewProfileComponent},
  {path:'changePassword',component:ChangePasswordComponent},
  {path:'pageNotFound',component:PageNotFoundComponent},// related to header don't change it.
  {path:'createAccount/:type',component:CreateAccountComponent},
  {path:'adminResetPassword', component:AdminResetPasswordComponent},
  {path:'**', redirectTo:'pageNotFound',pathMatch:'full'}

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

