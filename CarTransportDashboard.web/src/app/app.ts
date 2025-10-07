import {Component, OnInit, signal} from '@angular/core';
import {RouterOutlet, RouterLink, Router} from '@angular/router';
import { AuthService } from './services/auth/auth';
import { UserStoreService } from './services/auth/user-store-service';
import {SessionAction} from './components/session-actions/session-action/session-action';
import {AsyncPipe} from '@angular/common';
import {NAV_ITEMS, NavItem} from './models/nav-items';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, SessionAction, AsyncPipe],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App  {
  protected readonly title = signal('CarTransportDashboard.web');
  mobileMenuOpen = false;
  visibleNavItems: NavItem[] = [];
  constructor(private auth: AuthService, protected userStore: UserStoreService, private router: Router) {}


  ngOnInit() {
    this.userStore.user$.subscribe(user => {
      const role = user?.roles[0]; // assuming single role for simplicity
      if(role!=undefined) {
        this.visibleNavItems = NAV_ITEMS.filter(item => item.roles.includes(role));
      }
    });
  }


  toggleMobileMenu() {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }
  handleLogout(): void {
    this.auth.logout().subscribe({
      next: () => {this.router.navigate(['/account/login']);
        this.visibleNavItems = [];
        },
      error: () => this.router.navigate(['/account/login'])
    });
  }



}
