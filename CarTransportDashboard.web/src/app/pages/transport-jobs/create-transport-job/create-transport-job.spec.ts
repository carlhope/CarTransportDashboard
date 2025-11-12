import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateTransportJob } from './create-transport-job';
import { TransportJobService } from '../../../services/transport-job/transport-job';
import { TransportJob } from '../../../models/transport-job';
import { CreateTransportJobForm } from '../../../components/create-transport-job-form/create-transport-job-form';
import { of, throwError } from 'rxjs';
import { JobStatus } from '../../../models/job-status';
import {Address} from '../../../models/Address';
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

describe('CreateTransportJob', () => {
  let component: CreateTransportJob;
  let fixture: ComponentFixture<CreateTransportJob>;
  let mockService: jasmine.SpyObj<TransportJobService>;

  beforeEach(() => {
    mockService = jasmine.createSpyObj('TransportJobService', ['create']);

    TestBed.configureTestingModule({
      imports: [CreateTransportJobForm, CreateTransportJob],
      declarations: [],
      providers: [
        { provide: TransportJobService, useValue: mockService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CreateTransportJob);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should call TransportJobService.create() with the submitted job', () => {
    const mockJob: TransportJob = {
      title: 'Test Job',
      description: 'Test Description',
      status: JobStatus.Available,
      pickupLocation: mockPickupLocation,
      dropoffLocation: mockDropoffLocation,
      scheduledDate: new Date().toISOString()
    };

    mockService.create.and.returnValue(of({ ...mockJob, id: '123' }));

    component.handleSubmit(mockJob);

    expect(mockService.create).toHaveBeenCalledWith(mockJob);
  });

  it('should log error if service call fails', () => {
    const mockJob: TransportJob = {
      title: 'Failing Job',
      description: 'Should fail',
      status: JobStatus.Available,
      pickupLocation: mockPickupLocation,
      dropoffLocation: mockDropoffLocation,
      scheduledDate: new Date().toISOString()
    };

    spyOn(console, 'error');
    mockService.create.and.returnValue(throwError(() => new Error('Server error')));

    component.handleSubmit(mockJob);

    expect(console.error).toHaveBeenCalledWith('Error creating job:', jasmine.any(Error));
  });
});

