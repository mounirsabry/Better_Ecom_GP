import { TestBed } from '@angular/core/testing';

import { ReadOnlyStatusService } from './read-only-status.service';

describe('ReadOnlyStatusService', () => {
  let service: ReadOnlyStatusService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ReadOnlyStatusService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
