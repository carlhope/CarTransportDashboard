import { TransportJob } from './transport-job';
import {fuelType} from './fuel-type';

export interface Vehicle {
  id?: string;
  make: string;
  model: string;
  registrationNumber: string;
  fuelType: fuelType;
  assignedJobs?: TransportJob[];
}


