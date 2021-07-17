import { TestBed } from '@angular/core/testing';

import { SubmitDepartmentPriorityListService } from './submit-department-priority-list.service';

describe('SubmitDepartmentPriorityListService', () => {
  let service: SubmitDepartmentPriorityListService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SubmitDepartmentPriorityListService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
