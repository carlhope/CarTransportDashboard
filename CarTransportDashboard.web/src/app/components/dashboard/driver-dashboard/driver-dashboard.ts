import {Component, computed, effect, Input, OnInit, signal, SimpleChanges} from '@angular/core';
import {UserModel} from '../../../models/user';
import { TransportJob } from '../../../models/transport-job';
import { TransportJobService } from '../../../services/transport-job/transport-job';
import {EarningsSummary} from '../../../models/Earnings';
import {JobTabs, JobList, JobDetail, JobMap, EarningsSummary as EarningsDisplay} from '../shared'
import {JobStatus} from '../../../models/job-status';
import {toSignal} from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-driver-dashboard',
  imports: [
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
  acceptedJobs = signal<TransportJob[]>([]);
  allocatedJobs = signal<TransportJob[]>([]);
  completedJobs = signal<TransportJob[]>([]);
  selectedTab = signal<JobStatus>(JobStatus.InProgress);// default to "Active"
  selectedJob = signal<TransportJob | null>(null);
  filteredJobs = computed(() => {
    // force dependency tracking
    const accepted = this.acceptedJobs();
    const allocated = this.allocatedJobs();
    const completed = this.completedJobs();

    switch (this.selectedTab()) {
      case JobStatus.InProgress: return accepted;
      case JobStatus.Allocated: return allocated;
      case JobStatus.Completed: return completed;
      default: return [];
    }
  });

  private readonly now = new Date();

  private readonly todayStart = new Date(
    this.now.getFullYear(),
    this.now.getMonth(),
    this.now.getDate()
  );

  private readonly weekStart = new Date(
    this.now.getFullYear(),
    this.now.getMonth(),
    this.now.getDate() - this.now.getDay()
  );

  private readonly thirtyDaysAgo = new Date(
    this.now.getFullYear(),
    this.now.getMonth(),
    this.now.getDate() - 30
  );
  earnings = computed(() => {
    const jobs = this.completedJobs().filter(j => j.completedAt);

    return {
      today: this.sumSince(jobs, this.todayStart),
      thisWeek: this.sumSince(jobs, this.weekStart),
      last30Days: this.sumSince(jobs, this.thirtyDaysAgo)
    };
  });



  constructor(private jobService: TransportJobService) {
    effect(() => {
      const jobs = this.filteredJobs();
      this.selectedJob.set(jobs[0] ?? null);
    });
  }

  ngOnInit(): void {
    this.refreshJobs();

  }

  acceptJob(jobId: string): void {
    console.log('Sending jobId:', jobId);

    this.jobService.acceptJob(jobId).subscribe({
      next: updatedJob => {
        this.refreshJobs();
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
        this.refreshJobs();
      },
      error: err => {
        console.error(`Failed to complete job ${jobId}:`, err.message);
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
        this.refreshJobs();
      },
      error: err => {
        console.error(`Failed to decline job ${jobId}:`, err.message);
      }
    });
  }
  onTabChange(status :JobStatus) {
    this.selectedTab.set(status);
  }

  onJobSelected(job: TransportJob) {
    this.selectedJob.set(job);
  }

  private sumSince(jobs: TransportJob[], cutoff: Date): number {
    return jobs
      .filter(job => new Date(job.completedAt!).getTime() >= cutoff.getTime())
      .reduce((sum, job) => sum + (job.payout ?? 0), 0);
  }
  refreshJobs() {
    this.jobService.getAcceptedJobs().subscribe(jobs => {
      this.acceptedJobs.set(jobs);
    });

    this.jobService.getAvailableJobsForDriver().subscribe(jobs => {
      this.allocatedJobs.set(jobs);
    });

    this.jobService.getCompletedJobs().subscribe(jobs => {
      this.completedJobs.set(jobs);
    });
  }


}
