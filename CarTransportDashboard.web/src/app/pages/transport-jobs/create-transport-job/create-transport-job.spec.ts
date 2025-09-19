import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateTransportJob } from './create-transport-job';
import { TransportJobService } from '../../../services/transport-job/transport-job';
import { TransportJob } from '../../../models/transport-job';
import { CreateTransportJobForm } from '../../../components/create-transport-job-form/create-transport-job-form';
import { of, throwError } from 'rxjs';


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
      status: 'Available',
      pickupLocation: 'Depot A',
      dropoffLocation: 'Warehouse B',
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
      status: 'Available',
      pickupLocation: 'X',
      dropoffLocation: 'Y',
      scheduledDate: new Date().toISOString()
    };

    spyOn(console, 'error');
    mockService.create.and.returnValue(throwError(() => new Error('Server error')));

    component.handleSubmit(mockJob);

    expect(console.error).toHaveBeenCalledWith('Error creating job:', jasmine.any(Error));
  });
});

