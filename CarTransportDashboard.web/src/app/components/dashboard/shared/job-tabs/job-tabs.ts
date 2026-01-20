import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
  selector: 'app-job-tabs',
  imports: [],
  templateUrl: './job-tabs.html',
  styleUrl: './job-tabs.scss'
})
export class JobTabs {
  @Input() activeTab: string = 'active';
  @Output() tabChange = new EventEmitter<string>();

  tabs = [
    { id: 'active', label: 'Active' },
    { id: 'upcoming', label: 'Upcoming' },
    { id: 'completed', label: 'Completed' }
  ];

  selectTab(tabId: string) {
    this.tabChange.emit(tabId);
  }
}
