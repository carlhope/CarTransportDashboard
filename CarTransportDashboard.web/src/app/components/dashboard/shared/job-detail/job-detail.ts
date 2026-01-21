import {Component, Input} from '@angular/core';
import {DatePipe} from '@angular/common';
import {TransportJob} from '../../../../models/transport-job';
import {JobStatusDisplay} from '../../../../models/job-status';

@Component({
  selector: 'app-job-detail',
  imports: [
    DatePipe
  ],
  templateUrl: './job-detail.html',
  styleUrl: './job-detail.scss'
})
export class JobDetail {
  @Input() job: TransportJob | null = null;
  statusDisplay = JobStatusDisplay;

}
