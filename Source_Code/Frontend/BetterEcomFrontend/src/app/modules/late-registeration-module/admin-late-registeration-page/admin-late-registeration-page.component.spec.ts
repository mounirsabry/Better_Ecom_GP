import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminLateRegisterationPageComponent } from './admin-late-registeration-page.component';

describe('AdminLateRegisterationPageComponent', () => {
  let component: AdminLateRegisterationPageComponent;
  let fixture: ComponentFixture<AdminLateRegisterationPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdminLateRegisterationPageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminLateRegisterationPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
