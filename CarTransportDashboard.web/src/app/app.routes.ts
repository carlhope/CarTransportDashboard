import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';
import { Vehicles } from './pages/vehicles/vehicles';
import { TransportJobs } from './pages/transport-jobs/transport-jobs';
import { Drivers } from './pages/drivers/drivers';
import { Reports } from './pages/reports/reports';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: Dashboard },
  { path: 'vehicles', component: Vehicles },
  { path: 'transport-jobs', component: TransportJobs },
  { path: 'drivers', component: Drivers },
  { path: 'reports', component: Reports }
];
