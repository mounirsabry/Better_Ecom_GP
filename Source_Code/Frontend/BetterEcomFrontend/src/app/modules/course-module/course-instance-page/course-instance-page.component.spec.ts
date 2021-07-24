import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseInstancePageComponent } from './course-instance-page.component';

describe('CourseInstancePageComponent', () => {
  let component: CourseInstancePageComponent;
  let fixture: ComponentFixture<CourseInstancePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CourseInstancePageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CourseInstancePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
