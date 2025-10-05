import {Component, OnInit} from '@angular/core';
import { TransportJobService } from '../../services/transport-job/transport-job';
import { TransportJob } from '../../models/transport-job';
import {TransportJobListItem} from '../../components/transport-job-list-item/transport-job-list-item';
import {MatList} from '@angular/material/list';
import {RouterLink, RouterOutlet} from '@angular/router';

@Component({
  selector: 'app-transport-jobs',
  imports: [TransportJobListItem, MatList, RouterOutlet, RouterLink],
  templateUrl: './transport-jobs.html',
  styleUrl: './transport-jobs.scss'
})
export class TransportJobs implements OnInit {
  jobs: TransportJob[] = [];

  constructor(private jobService: TransportJobService) {}

  ngOnInit(): void {
    this.jobService.getJobs().subscribe(jobs => {
      this.jobs = jobs;
    });
  }
}
