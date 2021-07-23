import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterStudentInstructorInACourseComponent } from './register-student-instructor-in-a-course.component';

describe('RegisterStudentInstructorInACourseComponent', () => {
  let component: RegisterStudentInstructorInACourseComponent;
  let fixture: ComponentFixture<RegisterStudentInstructorInACourseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RegisterStudentInstructorInACourseComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RegisterStudentInstructorInACourseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
