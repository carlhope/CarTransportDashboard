import {Component, OnInit, signal} from '@angular/core';
import {RouterOutlet, Router} from '@angular/router';
import { AuthService } from './services/auth/auth';
import { UserStoreService } from './services/auth/user-store-service';
import {NAV_ITEMS, NavItem} from './models/nav-items';
import {Header} from './components/layout/header/header';
import {MobileNavMenu} from './components/layout/mobile-nav-menu/mobile-nav-menu';
import {NavMenu} from './components/layout/nav-menu/nav-menu';
import {Footer} from './components/layout/footer/footer';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header, MobileNavMenu, NavMenu, Footer],
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
