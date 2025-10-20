import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DriverDashboard } from './driver-dashboard';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import {provideHttpClient} from '@angular/common/http';

describe('DriverDashboard', () => {
  let component: DriverDashboard;
  let fixture: ComponentFixture<DriverDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DriverDashboard],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DriverDashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
