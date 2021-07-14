import { TestBed } from '@angular/core/testing';

import { RegisterNewStudentOrInstructorService } from './register-new-student-or-instructor.service';

describe('RegisterNewStudentOrInstructorService', () => {
  let service: RegisterNewStudentOrInstructorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RegisterNewStudentOrInstructorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
