import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors} from "@angular/forms";
import {TransportJob} from '../../models/transport-job';
import {VehicleService} from '../../services/vehicle/vehicle';
import {JobStatus} from '../../models/job-status';
import {Vehicle} from '../../models/vehicle';

@Component({
  selector: 'app-create-transport-job-form',
    imports: [
        ReactiveFormsModule
    ],
  templateUrl: './create-transport-job-form.html',
  styleUrl: './create-transport-job-form.scss'
})
export class CreateTransportJobForm implements OnInit {
  @Output() submitJob = new EventEmitter<TransportJob>();
  jobForm: FormGroup;
  vehicles = [
    { id: '1', make: 'Toyota', model: 'Camry', registrationNumber: 'ABC123' },
    { id: '2', make: 'Honda', model: 'Civic', registrationNumber: 'XYZ789' }
  ];
  matchedVehicle: Vehicle | null = null;
  hasSearchedRegistration = false;
  vehicleSearchFailed = false;
  registrationSearch: FormControl;

  constructor(
    private fb: FormBuilder,
    private vehicleService: VehicleService
  ) {
    this.registrationSearch = this.fb.control('');
    this.jobForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      pickupLocation: ['', Validators.required],
      dropoffLocation: ['', Validators.required],
      scheduledDate: ['', Validators.required],
      useNewVehicle: false, // toggle
      assignedVehicleId: [''],
      assignedVehicle: this.fb.group({
        make: ['', Validators.required],
        model: ['', Validators.required],
        registrationNumber: ['', Validators.required]
      })

    }
      )

  }

  ngOnInit() {
    //debugger;
    console.log('CreateTransportJobForm initialized');
  }


  onSubmit() {
    console.log('CreateTransportJobForm submitted');
    console.log(this.jobForm);
    if (this.jobForm.valid) {
      console.log("form valid");
      console.log(this.jobForm.value);

      const job: TransportJob = {
        ...this.jobForm.value,
        status: JobStatus.Available,
        scheduledDate: new Date(this.jobForm.value.scheduledDate).toISOString(),
        id: '00000000-0000-0000-0000-000000000000' // Temporary ID should be removed when integrated with backend

      };
      if (!this.jobForm.value.useNewVehicle) {
        job.assignedVehicle = undefined;
        console.log(job.assignedVehicleId);
      } else {
        job.assignedVehicleId = undefined;
        console.log(job.assignedVehicle);
      }



      //debugger;
      this.submitJob.emit(job);
      this.jobForm.reset({
        status: JobStatus.Available,
        useNewVehicle: false
      });

    }
    else{
      this.jobForm.markAllAsTouched();
      return;
    }
  }
  lookupVehicle() {
    this.hasSearchedRegistration = true;
    const reg = this.registrationSearch.value?.trim();
    if (!reg) return;

    this.vehicleService.getVehicleByRegistration(reg).subscribe({
      next: (vehicle) => {
        if (vehicle) {
          this.matchedVehicle = vehicle;
          // TODO: Replace with form-level validator to avoid temporary patching
          this.jobForm.patchValue({assignedVehicle: vehicle});
          this.vehicleSearchFailed = false;
          this.jobForm.patchValue({ assignedVehicleId: vehicle.id });
        } else {
          this.matchedVehicle = null;
          this.vehicleSearchFailed = true;
        }
      },
      error: () => {
        this.matchedVehicle = null;
        this.vehicleSearchFailed = true;
      }
    });
  }


}
