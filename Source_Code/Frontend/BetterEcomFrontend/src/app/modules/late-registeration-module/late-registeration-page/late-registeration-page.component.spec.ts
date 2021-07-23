import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LateRegisterationPageComponent } from './late-registeration-page.component';

describe('LateRegisterationPageComponent', () => {
  let component: LateRegisterationPageComponent;
  let fixture: ComponentFixture<LateRegisterationPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LateRegisterationPageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LateRegisterationPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
