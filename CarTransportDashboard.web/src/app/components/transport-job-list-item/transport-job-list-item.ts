import { Component, Input , OnInit} from '@angular/core';
import{ TransportJob } from '../../models/transport-job';
import { MatIcon } from '@angular/material/icon';
import {JobStatus, JobStatusDisplay} from '../../models/job-status';
import {DatePipe} from '@angular/common';

@Component({
  selector: 'app-transport-job-list-item',
  imports: [MatIcon, DatePipe],
  templateUrl: './transport-job-list-item.html',
  styleUrl: './transport-job-list-item.scss'
})
export class TransportJobListItem implements OnInit {
  @Input() job!: TransportJob;
  jobStatusDisplay = JobStatusDisplay;
  ngOnInit():void {
  }
}

