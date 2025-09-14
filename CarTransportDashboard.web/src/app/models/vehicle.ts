import { TransportJob } from './transport-job';

export interface Vehicle {
  id?: string;
  make: string;
  model: string;
  registrationNumber: string;
  assignedJobs?: TransportJob[];
}


