import {Component, OnInit, signal} from '@angular/core';
import {EarningsSummary, JobDetail, JobList, JobMap, JobTabs} from "../shared";
import {TransportJob} from '../../../models/transport-job';
import {DispatcherModel} from '../../../models/user';
import {JobStatus} from '../../../models/job-status';

@Component({
  selector: 'app-dispatcher-dashboard',
  imports: [
    JobList,
    JobTabs
  ],
  templateUrl: './dispatcher-dashboard.html',
  styleUrl: './dispatcher-dashboard.scss'
})
export class DispatcherDashboard implements OnInit {

  availableJobs = signal<TransportJob[]>([]);
  selectedJob = signal<TransportJob|null>(null);
  allocatedDrivers = signal<DispatcherModel[]>([]);
  selectedDriver = signal<DispatcherModel|null>(null);
  protected readonly selectedTab = signal<JobStatus>(JobStatus.InProgress);

  ngOnInit(): void {
    //populate availableJobs and allocatedDrivers
  }

  assignDriverToJob(driver: DispatcherModel, job: TransportJob) {
    //assign driver to job
  }
  removeDriver(driver: DispatcherModel, job: TransportJob) {
    //remove driver from job
  }

  protected onTabChange($event: JobStatus) {

  }
  protected onSelectedJobChange($event: JobStatus) {

  }
  protected onSelectedDriverChange($event: JobStatus) {

  }

  protected readonly JobStatus = JobStatus;
}
