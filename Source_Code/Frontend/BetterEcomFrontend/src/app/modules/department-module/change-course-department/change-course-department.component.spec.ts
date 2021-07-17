import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeCourseDepartmentComponent } from './change-course-department.component';

describe('ChangeCourseDepartmentComponent', () => {
  let component: ChangeCourseDepartmentComponent;
  let fixture: ComponentFixture<ChangeCourseDepartmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChangeCourseDepartmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangeCourseDepartmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
