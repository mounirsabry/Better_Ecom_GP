import { TestBed } from '@angular/core/testing';

import { GeneralFeedService } from './general-feed.service';

describe('GeneralFeedService', () => {
  let service: GeneralFeedService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GeneralFeedService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
