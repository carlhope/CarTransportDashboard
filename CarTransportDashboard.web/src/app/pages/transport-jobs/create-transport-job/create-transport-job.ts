import { Component } from '@angular/core';
import { TransportJobService } from '../../../services/transport-job/transport-job';
import { TransportJob } from '../../../models/transport-job';
import {CreateTransportJobForm} from '../../../components/create-transport-job-form/create-transport-job-form';

@Component({
  selector: 'app-create-transport-job',
  imports: [
    CreateTransportJobForm
  ],
  templateUrl: './create-transport-job.html',
  styleUrl: './create-transport-job.scss'
})
export class CreateTransportJob{
  constructor(private jobService: TransportJobService) {
  }

  handleSubmit($event: TransportJob) {
    const payload = { dto: $event }; // wrap in 'dto'
    this.jobService.create(payload).subscribe({
      next: created => console.log('Created job:', created),
      error: err => console.error('Error creating job:', err)
    });
  }

}
