import {Component, EventEmitter, Input, Output} from '@angular/core';
import {JobStatus} from '../../../../models/job-status';

@Component({
  selector: 'app-job-tabs',
  imports: [],
  templateUrl: './job-tabs.html',
  styleUrl: './job-tabs.scss'
})
export class JobTabs {
  @Input() activeTab: string = 'active';
  @Output() tabChange = new EventEmitter<JobStatus>();

  tabs = [
    { id: JobStatus.InProgress, label: 'Active' },
    { id: JobStatus.Available, label: 'Upcoming' },
    { id: JobStatus.Completed, label: 'Completed' }
  ];

  selectTab(tabId: JobStatus) {
    this.tabChange.emit(tabId);
  }

}
