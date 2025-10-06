import {Component, OnInit} from '@angular/core';
import {UserStoreService} from '../../services/auth/user-store-service';
import {DriverDashboard} from '../../components/dashboard/driver-dashboard/driver-dashboard';
import {GuestDashboard} from '../../components/dashboard/guest-dashboard/guest-dashboard';
import {AdminDashboard} from '../../components/dashboard/admin-dashboard/admin-dashboard';

@Component({
  selector: 'app-dashboard',
  imports: [
    DriverDashboard,
    GuestDashboard,
    AdminDashboard
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  constructor( private userStore: UserStoreService) {}
  isAdmin: boolean = false;
  isDriver: boolean = false;
ngOnInit(): void {
  this.isAdmin = this.userStore.hasRole('Admin');
  this.isDriver = this.userStore.hasRole('Driver');
}


}
