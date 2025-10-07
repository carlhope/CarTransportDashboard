import {Component, Input, OnInit} from '@angular/core';
import {UserModel} from '../../../models/user';

@Component({
  selector: 'app-driver-dashboard',
  imports: [],
  templateUrl: './driver-dashboard.html',
  styleUrl: './driver-dashboard.scss'
})
export class DriverDashboard implements OnInit {
  @Input() driver: UserModel|null = null;

  ngOnInit(): void {
    console.log("DriverDashboard initialized");
  }

}
