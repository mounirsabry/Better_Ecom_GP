import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddCourseToDepartmentComponent } from './add-course-to-department.component';

describe('AddCourseToDepartmentComponent', () => {
  let component: AddCourseToDepartmentComponent;
  let fixture: ComponentFixture<AddCourseToDepartmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddCourseToDepartmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddCourseToDepartmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
