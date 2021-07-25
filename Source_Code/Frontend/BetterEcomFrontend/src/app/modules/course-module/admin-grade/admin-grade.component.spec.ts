import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminGradeComponent } from './admin-grade.component';

describe('AdminGradeComponent', () => {
  let component: AdminGradeComponent;
  let fixture: ComponentFixture<AdminGradeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdminGradeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminGradeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
