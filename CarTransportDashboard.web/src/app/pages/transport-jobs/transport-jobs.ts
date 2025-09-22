import {Component, OnInit} from '@angular/core';
import { TransportJobService } from '../../services/transport-job/transport-job';
import { TransportJob } from '../../models/transport-job';
import {NgFor, NgIf} from '@angular/common';
import {TransportJobListItem} from '../../components/transport-job-list-item/transport-job-list-item';
import {MatList, MatListItem} from '@angular/material/list';

@Component({
  selector: 'app-transport-jobs',
  imports: [NgIf, NgFor, TransportJobListItem, MatList, MatListItem],
  templateUrl: './transport-jobs.html',
  styleUrl: './transport-jobs.scss'
})
export class TransportJobs implements OnInit {
  jobs: TransportJob[] = [];

  constructor(private jobService: TransportJobService) {}

  ngOnInit(): void {
    this.jobService.getJobs().subscribe(jobs => {
      this.jobs = jobs;
      //debugger;
      console.log('data: ', this.jobs);
    });
  }
}
