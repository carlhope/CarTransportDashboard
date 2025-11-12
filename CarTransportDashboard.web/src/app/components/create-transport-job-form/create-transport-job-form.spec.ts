import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateTransportJobForm } from './create-transport-job-form';
import {ReactiveFormsModule} from '@angular/forms';
import { TransportJob } from '../../models/transport-job';
import {fuelType} from '../../models/fuel-type';
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

describe('CreateTransportJobForm', () => {
  let component: CreateTransportJobForm;
  let fixture: ComponentFixture<CreateTransportJobForm>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, CreateTransportJobForm],
      declarations: []
    }).compileComponents();

    fixture = TestBed.createComponent(CreateTransportJobForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the form with default values', () => {
    expect(component.jobForm).toBeTruthy();
    expect(component.jobForm.get('useNewVehicle')?.value).toBeFalse();

  });

  it('should mark form as invalid when required fields are empty', () => {
    component.jobForm.setValue({
      title: '',
      description: '',
      pickupLocation: mockPickupLocation,
      dropoffLocation: mockPickupLocation,
      scheduledDate: '',
      useNewVehicle: false,
      assignedVehicleId: '',
      assignedVehicle: {
        make: '',
        model: '',
        registrationNumber: '',
        fuelType: fuelType.Diesel
      }
    });


    expect(component.jobForm.valid).toBeFalse();
  });

  it('should emit a valid TransportJob on submit', () => {
    spyOn(component.submitJob, 'emit');

    component.jobForm.setValue({
      title: 'Test Job',
      description: 'Test Description',
      pickupLocation: mockPickupLocation,
      dropoffLocation: mockDropoffLocation,
      scheduledDate: '2025-09-20',
      useNewVehicle: false,
      assignedVehicleId: 'v1',
      assignedVehicle: {
        make: '',
        model: '',
        registrationNumber: '',
        fuelType: fuelType.Diesel
      }
    });


    component.onSubmit();

    expect(component.submitJob.emit).toHaveBeenCalledWith(jasmine.objectContaining({
      title: 'Test Job',
      scheduledDate: new Date('2025-09-20').toISOString()
    }));
  });

  it('should not emit if form is invalid', () => {
    spyOn(component.submitJob, 'emit');

    component.jobForm.get('title')?.setValue('');
    component.onSubmit();

    expect(component.submitJob.emit).not.toHaveBeenCalled();
  });
});

