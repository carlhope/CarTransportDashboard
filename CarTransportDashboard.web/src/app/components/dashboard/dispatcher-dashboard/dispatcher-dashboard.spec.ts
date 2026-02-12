import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DispatcherDashboard } from './dispatcher-dashboard';

describe('DispatcherDashboard', () => {
  let component: DispatcherDashboard;
  let fixture: ComponentFixture<DispatcherDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DispatcherDashboard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DispatcherDashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
