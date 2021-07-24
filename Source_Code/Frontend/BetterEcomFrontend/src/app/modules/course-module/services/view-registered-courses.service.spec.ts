import { TestBed } from '@angular/core/testing';

import { ViewRegisteredCoursesService } from './view-registered-courses.service';

describe('ViewRegisteredCoursesService', () => {
  let service: ViewRegisteredCoursesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ViewRegisteredCoursesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
