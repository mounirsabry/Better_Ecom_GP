import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstructorHomePageComponent } from './instructor-home-page.component';

describe('InstructorHomePageComponent', () => {
  let component: InstructorHomePageComponent;
  let fixture: ComponentFixture<InstructorHomePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InstructorHomePageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InstructorHomePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
