import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TransportJobService } from './transport-job';
import { ModelMapperService } from '../model-mapper/model-mapper';
import {TransportJob } from '../../models/transport-job';
import { JobStatus } from '../../models/job-status';
import {Address} from '../../models/Address';
const mockPickupLocation: Address = {
  companyName: 'Acme Supplies Ltd',
  addressLine1: 'Unit 4, Acme Business Park',
  addressLine2: 'Warehouse Entrance',
  locality: 'Stoke-on-Trent',
  postalCode: 'ST1 1AA',
  country: 'GB',
  lat: 53.0027,
  lng: -2.1794,
  formatted: 'Acme Supplies Ltd, Unit 4, Acme Business Park, Stoke-on-Trent ST1 1AA, UK'
};

const mockDropoffLocation: Address = {
  companyName: 'Derby Distribution Hub',
  addressLine1: '456 Industrial Estate',
  addressLine2: 'Loading Bay 3',
  locality: 'Derby',
  postalCode: 'DE1 2BB',
  country: 'GB',
  lat: 52.9225,
  lng: -1.4746,
  formatted: 'Derby Distribution Hub, 456 Industrial Estate, Derby DE1 2BB, UK'
};
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
      pickupLocation: mockPickupLocation,
      dropoffLocation: mockDropoffLocation,
      scheduledDate: new Date('2025-09-19T10:00:00').toISOString(),
      status: JobStatus.Available
    };

    mapperSpy.toTransportJob.and.returnValue(mappedJob);

    service.getJobs().subscribe(jobs => {
      expect(jobs.length).toBe(1);
      expect(jobs[0]).toEqual(mappedJob);
      expect(mapperSpy.toTransportJob).toHaveBeenCalledWith(mockRawJobs[0]);
    });

    const req = httpMock.expectOne('https://localhost:7286/api/transportjobs');
    expect(req.request.method).toBe('GET');
    req.flush(mockRawJobs);
  });
});
