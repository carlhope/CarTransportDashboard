import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {TransportJob} from '../../models/transport-job';
import {TransportJobService} from '../../services/transport-job/transport-job';
import {JobStatus} from '../../models/job-status';

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
  constructor(
    private fb: FormBuilder
  ) {
    this.jobForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      status: [JobStatus.Available, Validators.required],
      pickupLocation: ['', Validators.required],
      dropoffLocation: ['', Validators.required],
      scheduledDate: ['', Validators.required],
      useNewVehicle: false, // toggle
      assignedVehicleId: [''],
      assignedVehicle: this.fb.group({
        make: ['', Validators.required],
        model: [''],
        registrationNumber: ['', Validators.required]
      })
    });
  }

  ngOnInit() {
    //debugger;
    console.log('CreateTransportJobForm initialized');
  }


  onSubmit() {
    if (this.jobForm.valid) {

      const job: TransportJob = {
        ...this.jobForm.value,
        scheduledDate: new Date(this.jobForm.value.scheduledDate).toISOString(),
        id: '00000000-0000-0000-0000-000000000000' // Temporary ID, should be removed when integrated with backend

      };
      if (!this.jobForm.value.useNewVehicle) {
        job.assignedVehicle = undefined;
      } else {
        job.assignedVehicleId = undefined;
      }

      //debugger;
      this.submitJob.emit(job);
      this.jobForm.reset({
        status: JobStatus.Available,
        useNewVehicle: false
      });

    }
  }


}
