import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TransportJobService } from './transport-job';
import { ModelMapperService } from '../model-mapper/model-mapper';
import { JobStatus, TransportJob } from '../../models/transport-job';

describe('TransportJobService', () => {
  let service: TransportJobService;
  let httpMock: HttpTestingController;
  let mapperSpy: jasmine.SpyObj<ModelMapperService>;

  beforeEach(() => {
    mapperSpy = jasmine.createSpyObj('ModelMapperService', ['toTransportJob']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        TransportJobService,
        { provide: ModelMapperService, useValue: mapperSpy }
      ]
    });

    service = TestBed.inject(TransportJobService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch jobs and map them', () => {
    const mockRawJobs = [{ id: '1', title: 'Test Job' }];
    const mappedJob: TransportJob = {
      id: '1',
      title: 'Test Job',
      description: 'Deliver medical supplies',
      pickupLocation: 'Warehouse A',
      dropoffLocation: 'Clinic B',
      scheduledDate: new Date('2025-09-19T10:00:00').toISOString(),
      status: 'Available' as JobStatus
    };

    mapperSpy.toTransportJob.and.returnValue(mappedJob);

    service.getJobs().subscribe(jobs => {
      expect(jobs.length).toBe(1);
      expect(jobs[0]).toEqual(mappedJob);
      expect(mapperSpy.toTransportJob).toHaveBeenCalledWith(mockRawJobs[0]);
    });

    const req = httpMock.expectOne('http://localhost:5176/api/transportjobs');
    expect(req.request.method).toBe('GET');
    req.flush(mockRawJobs);
  });
});
