import {Component, OnInit, signal} from '@angular/core';
import {RouterOutlet, RouterLink, Router} from '@angular/router';
import { AuthService } from './services/auth/auth';
import { UserStoreService } from './services/auth/user-store-service';
import {SessionAction} from './components/session-actions/session-action/session-action';
import {AsyncPipe} from '@angular/common';
import {SessionReadyService} from './services/auth/session-ready';
import {filter, take} from 'rxjs/operators';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, SessionAction, AsyncPipe],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly title = signal('CarTransportDashboard.web');
  mobileMenuOpen = false;
  constructor(private auth: AuthService, protected userStore: UserStoreService, private router: Router, private sessionReadyServive: SessionReadyService) {}

  //ngOnInit() {
  //  this.auth.refresh().subscribe({
   //   next: user => this.userStore.setUser(user),
   //  error: () => this.userStore.clearUser()
   // });
 //}
  ngOnInit() {
    this.sessionReadyServive.sessionReady$
      .pipe(filter(ready => ready), take(1))
      .subscribe(() => {
        // Now safe to call other services that rely on the token
        //this.dashboardService.loadInitialData().subscribe(...);
      });
  }




  toggleMobileMenu() {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }
  handleLogout(): void {
    this.auth.logout().subscribe({
      next: () => this.router.navigate(['/account/login']),
      error: () => this.router.navigate(['/account/login'])
    });
  }



}
