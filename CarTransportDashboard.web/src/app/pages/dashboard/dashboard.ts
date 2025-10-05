import { Component } from '@angular/core';
import {UserStoreService} from '../../services/auth/user-store-service';

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard {
  constructor( private userStore: UserStoreService) {}
  isAdmin: boolean = false;
  isDriver: boolean = false;
NgOnInit(): void {
  this.isAdmin = this.userStore.hasRole('Admin');
  this.isDriver = this.userStore.hasRole('Driver');
}


}
