import { TestBed } from '@angular/core/testing';

import { CourseFeedService } from './course-feed.service';

describe('CourseFeedService', () => {
  let service: CourseFeedService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CourseFeedService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
