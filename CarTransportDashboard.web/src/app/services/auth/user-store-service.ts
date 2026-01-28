import {Injectable, signal} from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { UserModel } from '../../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserStoreService {
  private _user = signal<UserModel | null>(null);
  user = this._user.asReadonly();
  public csrfToken: string | null = null;

  constructor() {
    this.csrfToken = localStorage.getItem("csrfToken");
  }

  setUser(user: UserModel) {
    this._user.set(user);
    if (user.csrfToken) {
      localStorage.setItem('csrfToken', user.csrfToken);
      const SEVEN_DAYS_MS = 7 * 24 * 60 * 60 * 1000;
      const refreshExpiryMs = Date.now() + SEVEN_DAYS_MS;
      localStorage.setItem('refreshExpiry', String(refreshExpiryMs));

    }
  }

  clearUser() {
    this._user.set(null);
    localStorage.removeItem('csrfToken');
    localStorage.removeItem('refreshExpiry');
  }

  get roles(): string[] {
    return this.user()?.roles ?? [];
  }

  hasRole(role: string): boolean {
    return this.roles.includes(role);
  }

  get isLoggedIn(): boolean {
    return !!this.user();
  }
}
