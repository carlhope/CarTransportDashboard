import {Component, EventEmitter, Input, Output} from '@angular/core';
import {RouterLink} from '@angular/router';
import {NavItem} from '../../../models/nav-items';
import {UserStoreService} from '../../../services/auth/user-store-service';
import {SessionAction} from '../../session-actions/session-action/session-action';

@Component({
  selector: 'app-mobile-nav-menu',
  imports: [
    RouterLink,
    SessionAction
  ],
  templateUrl: './mobile-nav-menu.html',
  styleUrl: './mobile-nav-menu.scss'
})
export class MobileNavMenu {
  @Input() visibleNavItems: NavItem[] = [];
  @Output() onLogout = new EventEmitter<void>();
  constructor(protected userStore: UserStoreService) {
  }
  handleLogout(): void {
    this.onLogout.emit();
  }


}
