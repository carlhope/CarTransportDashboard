import { Injectable } from '@angular/core';
import { Vehicle } from '../../models/vehicle';
import { MinimalUser } from '../../models/minimal-user';
import { TransportJob } from '../../models/transport-job';

@Injectable({
  providedIn: 'root'
})
export class ModelMapperService {
  toVehicle(data: any): Vehicle {
    return {
      id: data.id,
      make: data.make,
      model: data.model,
      registrationNumber: data.registrationNumber,
      fuelType: data.fuelType,
      assignedJobs: Array.isArray(data.assignedJobs)
        ? data.assignedJobs.map((job: any) => this.toTransportJob(job))
        : undefined
    };
  }

  toMinimalUser(data: any): MinimalUser {
    return {
      id: data.id,
      fullName: data.fullName,
      email: data.email
    };
  }

  toTransportJob(data: any): TransportJob {
    return {
      id: data.id,
      title: data.title,
      description: data.description,
      status: data.status,
      pickupLocation: data.pickupLocation,
      dropoffLocation: data.dropoffLocation,
      scheduledDate: data.scheduledDate,
      assignedVehicleId: data.assignedVehicleId,
      assignedVehicle: data.assignedVehicle ? this.toVehicle(data.assignedVehicle) : undefined,
      assignedDriverId: data.assignedDriverId,
      assignedDriver: data.assignedDriver ? this.toMinimalUser(data.assignedDriver) : undefined
    };
  }
}

