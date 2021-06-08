import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth-module/login/login.component';
import { StartPageComponent } from './start-page/start-page.component';

const routes: Routes = [
  {path:'',component:StartPageComponent},
  {path:'login/:type',component:LoginComponent},
  {path:'adminLogin',redirectTo:'/login',pathMatch:'full'}

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

export const routingComponents =[StartPageComponent]
