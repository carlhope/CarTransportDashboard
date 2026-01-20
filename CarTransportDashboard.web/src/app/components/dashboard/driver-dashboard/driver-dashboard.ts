import {Component, Input, OnInit} from '@angular/core';
import {UserModel} from '../../../models/user';
import { TransportJob } from '../../../models/transport-job';
import { TransportJobService } from '../../../services/transport-job/transport-job';
import {CurrencyPipe, DatePipe, DecimalPipe} from '@angular/common';
import {EarningsSummary} from '../../../models/Earnings';
import {JobTabs, JobList, JobDetail, JobMap, EarningsSummary as EarningsDisplay} from '../shared'
import {catchError, Observable, throwError} from 'rxjs';
import {DurationPipe} from '../../../pipes/duration/duration-pipe';

@Component({
  selector: 'app-driver-dashboard',
  imports: [
    DatePipe,
    CurrencyPipe,
    DecimalPipe,
    DurationPipe,
    JobList,
    JobTabs,
    JobDetail,
    JobMap,
    EarningsDisplay,
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
    today: 0,
    thisWeek: 0,
    last30Days: 0
  };

  constructor(private jobService: TransportJobService) {
  }

  ngOnInit(): void {
    console.log("DriverDashboard initialized");

    this.refreshData()

  }

  private refreshData(): void {
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
      this.loadEarnings();
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
    //cancelJob is admin only on backend. retained for re-use when admin dashboard created. To be replaced with requestCancellation
    this.jobService.cancelJob(jobId).subscribe({
      next: updatedJob => {
        console.log(`Job ${jobId} cancelled.`);
        this.refreshData();
      },
      error: err => {
        console.error(`Failed to cancel job ${jobId}:`, err.message);
      }
    });
  }
  requestCancellation(jobId: string): void {
    alert("Your cancellation request has been noted. An admin will review it shortly.");
    console.log(`Driver requested cancellation for job ${jobId}`);
    //full implementation will be added once admin workflow is established.
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
    const now = new Date();
    const todayStart = new Date(now.getFullYear(), now.getMonth(), now.getDate());
    const weekStart = new Date(now);
    weekStart.setDate(now.getDate() - now.getDay()); // Sunday as start of week
    const thirtyDaysAgo = new Date(now);
    thirtyDaysAgo.setDate(now.getDate() - 30);

    this.earnings.today = this.sumPayoutsSince(todayStart);
    this.earnings.thisWeek = this.sumPayoutsSince(weekStart);
    this.earnings.last30Days = this.sumPayoutsSince(thirtyDaysAgo);
  }

  private sumPayoutsSince(cutoffDate: Date): number {
    const jobs = this.completedJobs.filter(job => job.completedAt);
    return jobs
      .filter(job => {
        if (!job.completedAt) return false; // skip if undefined
        const completed = new Date(job.completedAt);
        return completed.getTime() >= cutoffDate.getTime();
      })
      .reduce((sum, job) => sum + (job.payout || 0), 0);
  }
}
