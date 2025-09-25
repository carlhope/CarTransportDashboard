import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';
import { Vehicles } from './pages/vehicles/vehicles';
import { TransportJobs } from './pages/transport-jobs/transport-jobs';
import { Drivers } from './pages/drivers/drivers';
import { Reports } from './pages/reports/reports';
import {CreateTransportJob} from './pages/transport-jobs/create-transport-job/create-transport-job';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: Dashboard },
  { path: 'vehicles', component: Vehicles },
  { path: 'transport-jobs', component: TransportJobs},
  {path: 'transport-jobs/create', component: CreateTransportJob },
  { path: 'drivers', component: Drivers },
  { path: 'reports', component: Reports }
];
