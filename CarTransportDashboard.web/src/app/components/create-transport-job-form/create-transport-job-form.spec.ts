import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateTransportJobForm } from './create-transport-job-form';
import {ReactiveFormsModule} from '@angular/forms';

import { TransportJob } from '../../models/transport-job';
import {fuelType} from '../../models/fuel-type';

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
      pickupLocation: '',
      dropoffLocation: '',
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
      pickupLocation: 'Location A',
      dropoffLocation: 'Location B',
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

