import {Component, Input, OnInit} from '@angular/core';
import {UserModel} from '../../../models/user';
import { TransportJob } from '../../../models/transport-job';
import { TransportJobService } from '../../../services/transport-job/transport-job';
import {DatePipe} from '@angular/common';

@Component({
  selector: 'app-driver-dashboard',
  imports: [
    DatePipe
  ],
  templateUrl: './driver-dashboard.html',
  styleUrl: './driver-dashboard.scss'
})

export class DriverDashboard implements OnInit {
  @Input() driver: UserModel | null = null;

  acceptedJobs: TransportJob[] = [];
  availableJobs: TransportJob[] = [];
  completedJobs: TransportJob[] = [];

  constructor(private jobService: TransportJobService) {}

  ngOnInit(): void {
    console.log("DriverDashboard initialized");

    this.loadAcceptedJobs();
    this.loadAvailableJobs();
    this.loadCompletedJobs();
  }

  private loadAcceptedJobs(): void {
    this.jobService.getAcceptedJobs().subscribe(jobs => {
      this.acceptedJobs = jobs;
      console.log('Accepted jobs:', jobs);
    });
  }

  private loadAvailableJobs(): void {
    this.jobService.getAvailableJobsForDriver().subscribe(jobs => {
      this.availableJobs = jobs;
      console.log('Available jobs:', jobs);
    });
  }

  private loadCompletedJobs(): void {
    this.jobService.getCompletedJobs(30).subscribe(jobs => {
      this.completedJobs = jobs;
      console.log('Completed jobs:', jobs);
    });
  }
  completeJob(jobId: string): void {
    console.log(`Complete job triggered for ID: ${jobId}`);
  }

  cancelJob(jobId: string): void {
    console.log(`Cancel job triggered for ID: ${jobId}`);
  }

  acceptJob(jobId: string): void {
    console.log(`Accept job triggered for ID: ${jobId}`);
  }

  declineJob(jobId: string): void {
    console.log(`Decline job triggered for ID: ${jobId}`);
  }

}
