import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DropCourseComponent } from './drop-course.component';

describe('DropCourseComponent', () => {
  let component: DropCourseComponent;
  let fixture: ComponentFixture<DropCourseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DropCourseComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DropCourseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
