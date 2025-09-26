import {Component, OnInit, signal} from '@angular/core';
import {RouterOutlet, RouterLink, Router} from '@angular/router';
import { AuthService } from './services/auth/auth';
import { UserStoreService } from './services/auth/user-store-service';
import {SessionAction} from './components/session-actions/session-action/session-action';
import {AsyncPipe} from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, SessionAction, AsyncPipe],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly title = signal('CarTransportDashboard.web');
  mobileMenuOpen = false;
  constructor(private auth: AuthService, protected userStore: UserStoreService, private router: Router) {}

  ngOnInit() {
    this.auth.refresh().subscribe({
      next: user => this.userStore.setUser(user),
      error: () => this.userStore.clearUser()
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
