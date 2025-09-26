import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Router, RouterLink} from '@angular/router';
import { AuthService } from '../../../services/auth/auth';
import {MatIcon} from '@angular/material/icon';
import {MatButton} from '@angular/material/button';
import {UserModel} from '../../../models/user';

@Component({
  selector: 'app-session-action',
  imports: [
    MatIcon,
    RouterLink,
    MatButton
  ],
  templateUrl: './session-action.html',
  styleUrl: './session-action.scss'
})
export class SessionAction {
  @Input() user: UserModel | null = null;
  @Output() onLogout = new EventEmitter<void>();



  logout(): void {
    this.onLogout.emit();
  }
}
