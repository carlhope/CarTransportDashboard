import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';
import { Vehicles } from './pages/vehicles/vehicles';
import { TransportJobs } from './pages/transport-jobs/transport-jobs';
import { Drivers } from './pages/drivers/drivers';
import { Reports } from './pages/reports/reports';
import {CreateTransportJob} from './pages/transport-jobs/create-transport-job/create-transport-job';
import {Login} from './pages/auth/login/login';
import {Register} from './pages/auth/register/register';
import {authGuard} from './guards/auth/auth-guard';
import {roleGuard} from './guards/roles/role-guard';
import {guestGuard} from './guards/guest/guest-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: Dashboard },
  { path: 'vehicles', component: Vehicles, canActivate: [authGuard] },
  { path: 'transport-jobs', component: TransportJobs},
  {path: 'transport-jobs/create', component: CreateTransportJob },
  { path: 'drivers', component: Drivers, canActivate: [roleGuard], data: {roles:['Admin', 'Dispatcher']} },
  { path: 'reports', component: Reports },
  {path: 'account/login', component: Login, canActivate: [guestGuard] },
  {path: 'account/register', component: Register, canActivate: [guestGuard] },
];
