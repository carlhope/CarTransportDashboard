import {Component, Input} from '@angular/core';
import {DatePipe} from '@angular/common';
import {TransportJob} from '../../../../models/transport-job';
import {JobStatus, JobStatusDisplay} from '../../../../models/job-status';
import {Button} from "../../../ui/button/button";

@Component({
  selector: 'app-job-detail',
  imports: [
    DatePipe,
    Button
  ],
  templateUrl: './job-detail.html',
  styleUrl: './job-detail.scss'
})
export class JobDetail {
  @Input() job: TransportJob | null = null;
  statusDisplay = JobStatusDisplay;
  // Default tab
  activeTab: 'summary' |'vehicle' | 'pickup' | 'dropoff' = 'summary';

  get actions(): JobAction[] {
    if (!this.job) return [];

    switch (this.job.status) {
      case JobStatus.Allocated:
        return [
          { label: 'Accept', variant: 'primary', action: 'accept' },
          { label: 'Decline', variant: 'danger', action: 'decline' }
        ];

      case JobStatus.InProgress:
        return [
          { label: 'Complete', variant: 'primary', action: 'complete' },
          { label: 'Cancel', variant: 'danger', action: 'cancel' }
        ];

      case JobStatus.Completed:
        return [];//empty as no actions available after completion

      default:
        return [];
    }
  }
  onAction(action: string) {
    switch (action) {
      case 'accept':
        this.acceptJob();
        break;

      case 'decline':
        this.declineJob();
        break;

      case 'complete':
        this.completeJob();
        break;

      case 'cancel':
        this.cancelJob();
        break;
    }
  }
  //placeholders
  acceptJob() {
    console.log('Accept job');
  }

  declineJob() {
    console.log('Decline job');
  }

  completeJob() {
    console.log('Complete job');
  }

  cancelJob() {
    console.log('Cancel job');
  }

}
type JobAction = {
  label: string;
  variant: 'primary' | 'secondary' | 'danger';
  action: string;
};
