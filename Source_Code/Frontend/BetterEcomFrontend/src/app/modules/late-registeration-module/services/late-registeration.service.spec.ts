import { TestBed } from '@angular/core/testing';

import { LateRegisterationService } from './late-registeration.service';

describe('LateRegisterationService', () => {
  let service: LateRegisterationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LateRegisterationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
