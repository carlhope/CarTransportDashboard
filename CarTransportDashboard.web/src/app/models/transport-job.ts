import { Vehicle } from './vehicle';
import {MinimalUser} from './minimal-user';

export type JobStatus = 'Available' | 'InProgress' | 'Completed';

export interface TransportJob {
  id?: string;
  title: string;
  description: string;
  status: JobStatus;
  pickupLocation: string;
  dropoffLocation: string;
  scheduledDate: string; // ISO string format

  assignedVehicleId?: string;
  assignedVehicle?: Vehicle;

  assignedDriverId?: string;
  assignedDriver?: MinimalUser;
}

