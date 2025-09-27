import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { TransportJobs } from './transport-jobs';
import {TransportJobService} from '../../services/transport-job/transport-job';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('TransportJobs', () => {
  let component: TransportJobs;
  let fixture: ComponentFixture<TransportJobs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransportJobs],
      providers: [
        TransportJobService,
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting()
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(TransportJobs);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    //expect(component).toBeTruthy();
  });
});
