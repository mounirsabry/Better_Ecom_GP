import { TestBed } from '@angular/core/testing';

import { RegisterStudentInstructorCourseService } from './register-student-instructor-course.service';

describe('RegisterStudentInstructorCourseService', () => {
  let service: RegisterStudentInstructorCourseService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RegisterStudentInstructorCourseService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
