import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {DurationPipe} from '../../../../pipes/duration/duration-pipe';
import {TransportJob} from '../../../../models/transport-job';
import {DriverModel} from '../../../../models/user';

@Component({
  selector: 'app-driver-list',
  imports: [
    DurationPipe
  ],
  templateUrl: './driver-list.html',
  styleUrl: './driver-list.scss'
})
export class DriverList {
  @Input() drivers: DriverModel[] = [];
  @Input() selectedDriver: DriverModel | null = null;

  @Output() readonly jobSelected = new EventEmitter<TransportJob>();
  @Output() readonly driverSelected = new EventEmitter<DriverModel>();

  selectJob(job: TransportJob) {
    this.jobSelected.emit(job);
  }
  selectDriver(d: DriverModel) {
    this.driverSelected.emit(d);
  }

}
