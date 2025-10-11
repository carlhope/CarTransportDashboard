import {Component, Input, OnInit} from '@angular/core';
import {UserModel} from '../../../models/user';
import { TransportJob } from '../../../models/transport-job';
import { TransportJobService } from '../../../services/transport-job/transport-job';
import {CurrencyPipe, DatePipe} from '@angular/common';
import {EarningsSummary} from '../../../models/Earnings';
import {catchError, Observable, throwError} from 'rxjs';

@Component({
  selector: 'app-driver-dashboard',
  imports: [
    DatePipe,
    CurrencyPipe
  ],
  templateUrl: './driver-dashboard.html',
  styleUrl: './driver-dashboard.scss'
})

export class DriverDashboard implements OnInit {
  @Input() driver: UserModel | null = null;

  acceptedJobs: TransportJob[] = [];
  availableJobs: TransportJob[] = [];
  completedJobs: TransportJob[] = [];
  currencyCode: string = 'GBP'; // Hardcoded for simplicity. Could be made dynamic based on locale.

  earnings: EarningsSummary = {
    //Hardcoded placeholder values for now
    today: 45.00,
    thisWeek: 342.25,
    last30Days: 1134.50
  };

  constructor(private jobService: TransportJobService) {}

  ngOnInit(): void {
    console.log("DriverDashboard initialized");

  this.refreshData()
  }
  private refreshData(): void {
    this.loadAcceptedJobs();
    this.loadAvailableJobs();
    this.loadCompletedJobs();
    this.loadEarnings();
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

  acceptJob(jobId: string): void {
    console.log(`Accept job triggered for ID: ${jobId}`);
    this.jobService.acceptJob(jobId).subscribe({
      next: updatedJob => {
        console.log('Job accepted:', updatedJob);
        this.refreshData();
        console.log(updatedJob);
      },
      error: err => {
        console.log('Error occurred:', err);
      }
    });
  }
  completeJob(jobId: string): void {
    this.jobService.completeJob(jobId).subscribe({
      next: updatedJob => {
        console.log(`Job ${jobId} marked as complete.`);
        this.refreshData();
      },
      error: err => {
        console.error(`Failed to complete job ${jobId}:`, err.message);
      }
    });
  }

  cancelJob(jobId: string): void {
    this.jobService.unassignJob(jobId).subscribe({
      next: updatedJob => {
        console.log(`Job ${jobId} cancelled.`);
        this.refreshData();
      },
      error: err => {
        console.error(`Failed to cancel job ${jobId}:`, err.message);
      }
    });
  }

  declineJob(jobId: string): void {
    this.jobService.unassignJob(jobId).subscribe({
      next: updatedJob => {
        console.log(`Job ${jobId} declined.`);
        this.refreshData();
      },
      error: err => {
        console.error(`Failed to decline job ${jobId}:`, err.message);
      }
    });
  }

  private loadEarnings(): void {
    console.log(`Load Earnings triggered for user: ${this.driver?.firstName} ${this.driver?.lastName}`);
  }

}
