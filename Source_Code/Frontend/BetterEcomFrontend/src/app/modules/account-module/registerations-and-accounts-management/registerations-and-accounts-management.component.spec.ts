import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterationsAndAccountsManagementComponent } from './registerations-and-accounts-management.component';

describe('RegisterationsAndAccountsManagementComponent', () => {
  let component: RegisterationsAndAccountsManagementComponent;
  let fixture: ComponentFixture<RegisterationsAndAccountsManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RegisterationsAndAccountsManagementComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RegisterationsAndAccountsManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
