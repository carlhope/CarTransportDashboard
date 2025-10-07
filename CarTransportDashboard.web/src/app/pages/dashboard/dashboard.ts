import {Component, OnInit} from '@angular/core';
import {UserStoreService} from '../../services/auth/user-store-service';
import {DriverDashboard} from '../../components/dashboard/driver-dashboard/driver-dashboard';
import {GuestDashboard} from '../../components/dashboard/guest-dashboard/guest-dashboard';
import {AdminDashboard} from '../../components/dashboard/admin-dashboard/admin-dashboard';
import {UserModel} from '../../models/user';
import {Observable, of} from 'rxjs';
import {AsyncPipe} from '@angular/common';

@Component({
  selector: 'app-dashboard',
  imports: [
    DriverDashboard,
    GuestDashboard,
    AdminDashboard,
    AsyncPipe
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  constructor( private userStore: UserStoreService) {}
  user:Observable<UserModel|null> = of(null);
  isAdmin: boolean = false;
  isDriver: boolean = false;
ngOnInit(): void {
  this.isAdmin = this.userStore.hasRole('Admin');
  this.isDriver = this.userStore.hasRole('Driver');
  this.user = this.userStore.user$;
}


}
