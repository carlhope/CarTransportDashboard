import {AfterViewInit, Component, computed, inject, OnChanges, OnInit, signal, SimpleChanges} from '@angular/core';
import {EarningsSummary, JobDetail, JobList, JobMap, JobTabs} from "../shared";
import {TransportJob} from '../../../models/transport-job';
import {DispatcherModel, DriverModel} from '../../../models/user';
import {DispatcherTab} from '../../../models/DispatcherTabs';
import {JobStatus} from '../../../models/job-status';
import {DispatcherTabs} from './components/dispatcher-tabs/dispatcher-tabs';
import {DriverList} from '../shared/driver-list/driver-list';
import {TransportJobService} from '../../../services/transport-job/transport-job';
import {DriverService} from '../../../services/driver/driver-service';
import {delay, forkJoin} from 'rxjs';

@Component({
  selector: 'app-dispatcher-dashboard',
  imports: [
    JobList,
    DispatcherTabs,
    DriverList
  ],
  templateUrl: './dispatcher-dashboard.html',
  styleUrl: './dispatcher-dashboard.scss'
})
export class DispatcherDashboard implements OnInit{

  availableJobs = signal<TransportJob[]>([]);
  selectedJob = signal<TransportJob|null>(null);
  allocatedDrivers = signal<DriverModel[]>([]);
  selectedDriver = signal<DriverModel|null>(null);
  protected readonly selectedTab = signal<DispatcherTab>("Jobs");
  jobCount = computed(() => this.availableJobs().length);
  driverCount = 25;

  constructor(private jobService: TransportJobService, private DriverService: DriverService) {
  }

  ngOnInit(): void {
    forkJoin({
      jobs: this.jobService.getAvailableJobs(),
      drivers: this.DriverService.getAll()
    }).subscribe(({jobs, drivers}) => {
      this.availableJobs.set(jobs);
      this.allocatedDrivers.set(drivers);
      console.log("drivers", this.allocatedDrivers());
    });

  }




  assignDriverToJob(driver: DispatcherModel, job: TransportJob) {
    //assign driver to job
  }
  removeDriver(driver: DispatcherModel, job: TransportJob) {
    //remove driver from job
  }

  protected onTabChange($event: DispatcherTab) {
    this.selectedTab.set($event);
  }
  protected onSelectedJobChange(job: TransportJob) {
    this.selectedJob.set(job)

  }
  protected onSelectedDriverChange(d: DriverModel) {
    this.selectedDriver.set(d)

  }

  protected readonly JobStatus = JobStatus;
}
