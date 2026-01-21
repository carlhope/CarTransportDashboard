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
  @Input() jobs: TransportJob[] = [];
  @Input() selectedJob: TransportJob | null = null;
  statusDisplay = JobStatusDisplay;

  @Output() jobSelected = new EventEmitter<TransportJob>();

  selectJob(job: TransportJob) {
    this.jobSelected.emit(job);
  }
}
