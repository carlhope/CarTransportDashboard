import {Component, OnInit} from '@angular/core';
import { TransportJobService } from '../../services/transport-job/transport-job';
import { TransportJob } from '../../models/transport-job';
import {NgFor, NgIf} from '@angular/common';

@Component({
  selector: 'app-transport-jobs',
  imports: [NgIf, NgFor],
  templateUrl: './transport-jobs.html',
  styleUrl: './transport-jobs.scss'
})
export class TransportJobs implements OnInit {
  jobs: TransportJob[] = [];

  constructor(private jobService: TransportJobService) {}

  ngOnInit(): void {
    this.jobService.getJobs().subscribe(jobs => {
      this.jobs = jobs;
      debugger;
      console.log(this.jobs);
    });
  }
}
