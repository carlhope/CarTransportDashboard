import { Vehicle } from './vehicle';
import {MinimalUser} from './minimal-user';
import { JobStatus } from './job-status';
import {Address} from './Address';



export interface TransportJob {
  id?: string;
  title: string;
  description: string;
  status: JobStatus;

  pickupLocation: Address;
  dropoffLocation: Address;
  scheduledDate: string;

  acceptedAt?: string;
  completedAt?: string;
  payout?: number;

  assignedVehicleId?: string;
  assignedVehicle?: Vehicle;

  assignedDriverId?: string;
  assignedDriver?: MinimalUser;

  //values calculated on backend and returned via API
  distanceInMiles?: number;
  estimatedTime?: string;
}


