import { HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/compiler';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthService } from './modules/auth-module/services/auth.service';
import { StartPageComponent } from './components/start-page/start-page.component';
import { AuthModule } from './modules/auth-module/auth.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AdminHomePageComponent } from './components/admin-home-page/admin-home-page.component';
import { InstructorHomePageComponent } from './components/instructor-home-page/instructor-home-page.component';
import { StudentHomePageComponent } from './components/student-home-page/student-home-page.component';
import { AccountModuleModule } from './modules/account-module/account-module.module';
import {InterceptorService} from './services/interceptors/interceptor.service'
@NgModule({

  declarations: [
    AppComponent,
    StartPageComponent,
    AdminHomePageComponent,
    InstructorHomePageComponent,
    StudentHomePageComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    AuthModule,
    HttpClientModule,
    ReactiveFormsModule,
    AccountModuleModule,

  ],
  providers: [{
    provide:HTTP_INTERCEPTORS,
    useClass: InterceptorService,
    multi:true
  }],
  bootstrap: [AppComponent]
})
export class AppModule { }