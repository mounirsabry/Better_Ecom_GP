import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterNewStudentOrInstructorComponent } from './register-new-student-or-instructor.component';

describe('RegisterNewStudentOrInstructorComponent', () => {
  let component: RegisterNewStudentOrInstructorComponent;
  let fixture: ComponentFixture<RegisterNewStudentOrInstructorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RegisterNewStudentOrInstructorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RegisterNewStudentOrInstructorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
