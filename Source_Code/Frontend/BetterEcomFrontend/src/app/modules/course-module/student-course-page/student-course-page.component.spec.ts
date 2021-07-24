import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentCoursePageComponent } from './student-course-page.component';

describe('StudentCoursePageComponent', () => {
  let component: StudentCoursePageComponent;
  let fixture: ComponentFixture<StudentCoursePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StudentCoursePageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentCoursePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
