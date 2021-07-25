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
import {InterceptorService} from './services/interceptors/interceptor.service';
import { ChooseDepartmentComponent } from './modules/department-module/choose-department/choose-department.component';
import { ViewDepartmentsCoursesComponent } from './modules/department-module/view-departments-courses/view-departments-courses.component';
import { AddCourseToDepartmentComponent } from './modules/department-module/add-course-to-department/add-course-to-department.component';
import { AdminDepartmentComponent } from './modules/department-module/admin-department/admin-department.component';
import { CourseInfoComponent } from './modules/department-module/course-info/course-info.component';
import { AddCourseInstanceComponent } from './modules/course-module/add-course-instance/add-course-instance.component';
import { AdminCoursePageComponent } from './modules/course-module/admin-course-page/admin-course-page.component';
import { RegisterStudentInstructorInACourseComponent } from './modules/course-module/register-student-instructor-in-a-course/register-student-instructor-in-a-course.component';
import { LateRegisterationPageComponent } from './modules/late-registeration-module/late-registeration-page/late-registeration-page.component';
import { AdminLateRegisterationPageComponent } from './modules/late-registeration-module/admin-late-registeration-page/admin-late-registeration-page.component';
import { StudentCoursePageComponent } from './modules/course-module/student-course-page/student-course-page.component';
import { GeneralFeedComponent } from './modules/feed-module/general-feed/general-feed.component';
import { CourseInstancePageComponent } from './modules/course-module/course-instance-page/course-instance-page.component';
import { InstructorCoursePageComponent } from './modules/course-module/instructor-course-page/instructor-course-page.component';
import { DropCourseComponent } from './modules/course-module/drop-course/drop-course.component';
import { StudentGradeComponent } from './modules/course-module/student-grade/student-grade.component';
import { AdminGradeComponent } from './modules/course-module/admin-grade/admin-grade.component';
import { ReadOnlyStatusComponent } from './modules/course-module/read-only-status/read-only-status.component';
import { CourseFeedComponent } from './modules/feed-module/course-feed/course-feed.component';
@NgModule({

  declarations: [
    AppComponent,
    StartPageComponent,
    AdminHomePageComponent,
    InstructorHomePageComponent,
    StudentHomePageComponent,
    ChooseDepartmentComponent,
    ViewDepartmentsCoursesComponent,
    AddCourseToDepartmentComponent,
    AdminDepartmentComponent,
    CourseInfoComponent,
    AddCourseInstanceComponent,
    AdminCoursePageComponent,
    RegisterStudentInstructorInACourseComponent,
    LateRegisterationPageComponent,
    AdminLateRegisterationPageComponent,
    StudentCoursePageComponent,
    GeneralFeedComponent,
    CourseInstancePageComponent,
    InstructorCoursePageComponent,
    DropCourseComponent,
    StudentGradeComponent,
    AdminGradeComponent,
    ReadOnlyStatusComponent,
    CourseFeedComponent


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
