import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RegisterModel, LoginModel, UserModel } from '../../models/user';
import {Observable, Observer, tap} from 'rxjs';
import { UserStoreService } from './user-store-service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly baseUrl = 'https://localhost:7286/api/auth';

  constructor(private http: HttpClient, private userStore: UserStoreService) {}
  private isLoggingIn:boolean=false;

  register(dto: RegisterModel): Observable<UserModel> {
    return this.http.post<UserModel>(`${this.baseUrl}/register`, dto)
      .pipe(tap(user => this.userStore.setUser(user)));
  }

  login(dto: LoginModel): Observable<UserModel> {
    return this.http.post<UserModel>(`${this.baseUrl}/login`, dto)
      .pipe(tap(user => this.userStore.setUser(user)));
  }
  googleLogin(idToken: string): Observable<UserModel> {
    return this.http.post<UserModel>(`${this.baseUrl}/google`, { idToken })
      .pipe(tap(user => this.userStore.setUser(user)));
  }


  refresh(): Observable<UserModel> {
    return this.http.post<UserModel>(`${this.baseUrl}/refresh`, {})
      .pipe(tap(user => this.userStore.setUser(user)));
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/logout`, {})
      .pipe(tap(() => this.userStore.clearUser()));
  }

}

