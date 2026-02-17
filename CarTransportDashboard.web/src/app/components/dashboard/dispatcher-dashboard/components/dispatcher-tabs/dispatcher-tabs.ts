import {Component, EventEmitter, Input, Output} from '@angular/core';
import {DispatcherTab} from '../../../../../models/DispatcherTabs';

@Component({
  selector: 'app-dispatcher-tabs',
  imports: [],
  templateUrl: './dispatcher-tabs.html',
  styleUrl: './dispatcher-tabs.scss'
})
export class DispatcherTabs {
  @Input() activeTab: DispatcherTab = "Jobs";
  @Input() jobCount = 0;
  @Input() driverCount = 0;

  @Output() readonly tabChange = new EventEmitter<DispatcherTab>();

  tabs:{id:DispatcherTab, label : string}[] = [
    { id: "Jobs", label: 'Jobs' },
    { id: "Drivers", label: 'Drivers' }
  ];

  selectTab(tabId: DispatcherTab):void {
    this.tabChange.emit(tabId);
  }
  getCount(tabId: DispatcherTab): number {
    const counts: Record<DispatcherTab, number> = {
      Jobs: this.jobCount,
      Drivers: this.driverCount
    };

    return counts[tabId];
  }



}
