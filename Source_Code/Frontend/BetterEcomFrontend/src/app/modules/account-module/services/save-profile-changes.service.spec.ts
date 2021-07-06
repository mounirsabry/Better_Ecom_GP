import { TestBed } from '@angular/core/testing';

import { SaveProfileChangesService } from './save-profile-changes.service';

describe('SaveProfileChangesService', () => {
  let service: SaveProfileChangesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SaveProfileChangesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
