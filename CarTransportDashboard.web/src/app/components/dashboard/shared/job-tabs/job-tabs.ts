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
  @Input() activeCount = 0;
  @Input() allocatedCount = 0;
  @Input() completedCount = 0;
  @Output() tabChange = new EventEmitter<JobStatus>();

  tabs = [
    { id: JobStatus.InProgress, label: 'Active' },
    { id: JobStatus.Allocated, label: 'Allocated' },
    { id: JobStatus.Completed, label: 'Completed' }
  ];

  selectTab(tabId: JobStatus) {
    this.tabChange.emit(tabId);
  }
  getCount(tabId: JobStatus): number {
    switch (tabId) {
      case JobStatus.InProgress:
        return this.activeCount;
      case JobStatus.Allocated:
        return this.allocatedCount;
      case JobStatus.Completed:
        return this.completedCount;
      default:
        return 0;
    }
  }


}
