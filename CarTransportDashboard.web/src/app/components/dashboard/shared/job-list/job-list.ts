import {Component, EventEmitter, Input, Output} from '@angular/core';
import {TransportJob} from '../../../../models/transport-job';
import {JobStatus, JobStatusDisplay} from '../../../../models/job-status';

@Component({
  selector: 'app-job-list',
  imports: [],
  templateUrl: './job-list.html',
  styleUrl: './job-list.scss'
})
export class JobList {
  //@Input() jobs: TransportJob[] = [];
  @Input() selectedJobId: string | null = null;
  statusDisplay = JobStatusDisplay;


  //temporary mock data
  jobs: TransportJob[] = [
    {
      id: '1',
      title: 'Airport Pickup',
      description: 'Collect vehicle from Manchester Airport and deliver to Stoke.',
      status: JobStatus.InProgress,
      scheduledDate: '2025-01-20T09:00:00Z',

      pickupLocation: {
        companyName: 'Manchester Airport',
        addressLine1: 'Terminal 1',
        locality: 'Manchester',
        postalCode: 'M90 1QX',
        country: 'UK'
      },

      dropoffLocation: {
        addressLine1: 'Campbell Road',
        locality: 'Stoke-on-Trent',
        postalCode: 'ST4 4DX',
        country: 'UK'
      },

      payout: 85,
      distanceInMiles: 44,
      estimatedTime: '1h 5m'
    },

    {
      id: '2',
      title: 'Astra Delivery',
      description: 'Deliver from depot to Crewe.',
      status: JobStatus.Available,
      scheduledDate: '2025-01-21T14:00:00Z',

      pickupLocation: {
        companyName: 'Hanley Depot',
        addressLine1: 'Leek Road',
        locality: 'Stoke-on-Trent',
        postalCode: 'ST1 3NF',
        country: 'UK'
      },

      dropoffLocation: {
        addressLine1: 'West Street',
        locality: 'Crewe',
        postalCode: 'CW1 3HX',
        country: 'UK'
      },

      payout: 42,
      distanceInMiles: 17,
      estimatedTime: '32m'
    },

    {
      id: '3',
      title: 'Vehicle Transport',
      description: 'Move vehicle from Birmingham to Liverpool.',
      status: JobStatus.Completed,
      scheduledDate: '2025-01-18T10:30:00Z',

      pickupLocation: {
        addressLine1: 'Digbeth',
        locality: 'Birmingham',
        postalCode: 'B5 6DY',
        country: 'UK'
      },

      dropoffLocation: {
        addressLine1: 'Edge Lane',
        locality: 'Liverpool',
        postalCode: 'L7 9NJ',
        country: 'UK'
      },

      payout: 120,
      distanceInMiles: 98,
      estimatedTime: '2h 10m',
      completedAt: '2025-01-18T13:00:00Z'
    }
  ];




  @Output() jobSelected = new EventEmitter<TransportJob>();

  selectJob(job: TransportJob) {
    this.jobSelected.emit(job);
  }
}
