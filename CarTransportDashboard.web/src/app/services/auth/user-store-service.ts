import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { UserModel } from '../../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserStoreService {
  private userSubject = new BehaviorSubject<UserModel | null>(null);
  user$ = this.userSubject.asObservable();
  public csrfToken: string | null = null;

  constructor() {
    this.csrfToken = localStorage.getItem("csrfToken");
  }

  setUser(user: UserModel) {
    this.userSubject.next(user);
    if (user.csrfToken) {
      localStorage.setItem('csrfToken', user.csrfToken);
      const SEVEN_DAYS_MS = 7 * 24 * 60 * 60 * 1000;
      const refreshExpiryMs = Date.now() + SEVEN_DAYS_MS;
      localStorage.setItem('refreshExpiry', String(refreshExpiryMs));

    }
  }

  clearUser() {
    this.userSubject.next(null);
    localStorage.removeItem('csrfToken');
    localStorage.removeItem('refreshExpiry');
  }

  get currentUser(): UserModel | null {
    return this.userSubject.value;
  }

  get roles(): string[] {
    return this.currentUser?.roles ?? [];
  }

  hasRole(role: string): boolean {
    return this.roles.includes(role);
  }

  get isLoggedIn(): boolean {
    return !!this.currentUser;
  }
}
