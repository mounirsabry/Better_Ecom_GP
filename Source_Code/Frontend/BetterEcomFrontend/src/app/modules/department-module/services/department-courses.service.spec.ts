import { TestBed } from '@angular/core/testing';

import { DepartmentCoursesService } from './department-courses.service';

describe('DepartmentCoursesService', () => {
  let service: DepartmentCoursesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DepartmentCoursesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
