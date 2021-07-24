import { TestBed } from '@angular/core/testing';

import { DropCourseServiceService } from './drop-course-service.service';

describe('DropCourseServiceService', () => {
  let service: DropCourseServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DropCourseServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
