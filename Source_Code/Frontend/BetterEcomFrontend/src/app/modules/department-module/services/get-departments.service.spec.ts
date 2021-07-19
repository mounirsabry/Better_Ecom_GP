import { TestBed } from '@angular/core/testing';

import { GetDepartmentsService } from './get-departments.service';

describe('GetDepartmentsService', () => {
  let service: GetDepartmentsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GetDepartmentsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
