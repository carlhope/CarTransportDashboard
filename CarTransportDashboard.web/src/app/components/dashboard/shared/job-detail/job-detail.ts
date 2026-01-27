import {Component, EventEmitter, Input, Output} from '@angular/core';
import {DatePipe, DecimalPipe} from '@angular/common';
import {DurationPipe} from '../../../../pipes/duration/duration-pipe';
import {TransportJob} from '../../../../models/transport-job';
import {JobStatus} from '../../../../models/job-status';
import {Button} from "../../../ui/button/button";
import {JobAction} from '../../../../models/shared/ui-actions';

@Component({
  selector: 'app-job-detail',
  imports: [
    DatePipe,
    Button,
    DurationPipe,
    DecimalPipe
  ],
  templateUrl: './job-detail.html',
  styleUrl: './job-detail.scss'
})
export class JobDetail {
  @Input() job: TransportJob | null = null;
  @Output() accept = new EventEmitter<string>();
  @Output() decline = new EventEmitter<string>();
  @Output() complete = new EventEmitter<string>();
  @Output() cancel = new EventEmitter<string>();
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
    if (!this.job?.id) return;
    switch (action) {
      case 'accept':
        this.accept.emit(this.job.id);
        break;

      case 'decline':
        this.decline.emit(this.job.id);
        break;

      case 'complete':
        this.complete.emit(this.job.id);
        break;

      case 'cancel':
        this.cancel.emit(this.job.id);
        break;
    }
  }

}

