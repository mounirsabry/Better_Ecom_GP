import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReadOnlyStatusComponent } from './read-only-status.component';

describe('ReadOnlyStatusComponent', () => {
  let component: ReadOnlyStatusComponent;
  let fixture: ComponentFixture<ReadOnlyStatusComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReadOnlyStatusComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReadOnlyStatusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
