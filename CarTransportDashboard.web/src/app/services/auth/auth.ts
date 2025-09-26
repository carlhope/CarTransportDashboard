import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {RegisterModel, LoginModel, UserModel, JwtPayload} from '../../models/user';
import { Observable } from 'rxjs';
import {jwtDecode} from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly baseUrl = 'http://localhost:5000/api/auth'; // adjust if needed

  constructor(private http: HttpClient) {}

  register(dto: RegisterModel): Observable<UserModel> {
    return this.http.post<UserModel>(`${this.baseUrl}/register`, dto, { withCredentials: true });
  }

  login(dto: LoginModel): Observable<UserModel> {
    return this.http.post<UserModel>(`${this.baseUrl}/login`, dto, { withCredentials: true });
  }

  refresh(): Observable<UserModel> {
    return this.http.post<UserModel>(`${this.baseUrl}/refresh`, {}, { withCredentials: true });
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/logout`, {}, { withCredentials: true });
  }

  getUserRoles(): string[] {
    const token = localStorage.getItem('accessToken');
    if (!token) return [];

    const decoded = jwtDecode<JwtPayload>(token);
    return Array.isArray(decoded.roles) ? decoded.roles : [decoded.roles];
  }

  hasRole(role: string): boolean {
    return this.getUserRoles().includes(role);
  }

}

