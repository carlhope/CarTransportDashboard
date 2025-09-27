import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { UserModel } from '../../models/user';

@Injectable({
  providedIn: 'root'
})

export class UserStoreService {
  private userSubject = new BehaviorSubject<UserModel | null>(null);
  user$ = this.userSubject.asObservable();

  setUser(user: UserModel) {
    this.userSubject.next(user);
    console.log("setting user", user);
    localStorage.setItem('accessToken', user.accessToken ?? 'no user');
    console.log('Stored token:', localStorage.getItem('accessToken'));
  }

  clearUser() {
    this.userSubject.next(null);
    localStorage.removeItem('accessToken');
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
