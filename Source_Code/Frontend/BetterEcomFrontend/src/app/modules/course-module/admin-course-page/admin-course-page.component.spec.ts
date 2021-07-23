import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminCoursePageComponent } from './admin-course-page.component';

describe('AdminCoursePageComponent', () => {
  let component: AdminCoursePageComponent;
  let fixture: ComponentFixture<AdminCoursePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdminCoursePageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminCoursePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
