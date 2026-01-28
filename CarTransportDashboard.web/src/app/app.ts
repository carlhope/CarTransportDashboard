import {Component, OnInit, signal, effect, Injector, runInInjectionContext} from '@angular/core';
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
  visibleNavItems = signal<NavItem[]>([]);
  constructor(private auth: AuthService, protected userStore: UserStoreService, private router: Router, private injector: Injector) {

  }


  ngOnInit() {
    runInInjectionContext(this.injector, () => {
      effect(() => {
        const user = this.userStore.user();
        const role = user?.roles[0];

        if (role) {
          this.visibleNavItems.set(
            NAV_ITEMS.filter(item => item.roles.includes(role))
          );

        } else {
          this.visibleNavItems.set([]);
        }
      });
    });

  }


  toggleMobileMenu() {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }
  handleLogout(): void {
    this.auth.logout().subscribe({
      next: () => {this.router.navigate(['/account/login']);
        this.visibleNavItems.set([]);
        },
      error: () => this.router.navigate(['/account/login'])
    });
  }
}
