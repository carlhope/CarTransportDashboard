import {Component, EventEmitter, Input, Output} from '@angular/core';
import {NavItem} from '../../../models/nav-items';
import {UserStoreService} from '../../../services/auth/user-store-service';
import {SessionAction} from '../../session-actions/session-action/session-action';
import {AsyncPipe} from '@angular/common';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-nav-menu',
  imports: [
    SessionAction,
    AsyncPipe,
    RouterLink
  ],
  templateUrl: './nav-menu.html',
  styleUrl: './nav-menu.scss'
})
export class NavMenu {
  @Input() visibleNavItems: NavItem[] = [];
  @Output() onLogout = new EventEmitter<void>();
  constructor(protected userStore: UserStoreService) {
  }
  handleLogout(): void {
    this.onLogout.emit();
  }
}
