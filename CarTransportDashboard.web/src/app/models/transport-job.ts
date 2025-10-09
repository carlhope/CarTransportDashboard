import { Vehicle } from './vehicle';
import {MinimalUser} from './minimal-user';
import { JobStatus } from './job-status';



export interface TransportJob {
  id?: string;
  title: string;
  description: string;
  status: JobStatus;

  pickupLocation: string;
  dropoffLocation: string;
  scheduledDate: string;

  acceptedAt?: string;
  completedAt?: string;
  payout?: number;

  assignedVehicleId?: string;
  assignedVehicle?: Vehicle;

  assignedDriverId?: string;
  assignedDriver?: MinimalUser;
}


