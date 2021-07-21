import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewDepartmentsCoursesComponent } from './view-departments-courses.component';

describe('ViewDepartmentsCoursesComponent', () => {
  let component: ViewDepartmentsCoursesComponent;
  let fixture: ComponentFixture<ViewDepartmentsCoursesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewDepartmentsCoursesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewDepartmentsCoursesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
