import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TransportJobs } from './transport-jobs';
import {TransportJobService} from '../../services/transport-job/transport-job';

describe('TransportJobs', () => {
  let component: TransportJobs;
  let fixture: ComponentFixture<TransportJobs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransportJobs, HttpClientTestingModule],
      providers: [TransportJobService]
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
