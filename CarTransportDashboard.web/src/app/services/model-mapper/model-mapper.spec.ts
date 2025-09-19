import { TestBed } from '@angular/core/testing';
import { ModelMapperService } from './model-mapper';
import { TransportJob } from '../../models/transport-job';
import { Vehicle } from '../../models/vehicle';
import { MinimalUser } from '../../models/minimal-user';

describe('ModelMapperService', () => {
  let service: ModelMapperService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ModelMapperService);
  });

  describe('toTransportJob', () => {
    it('should map raw data to TransportJob', () => {
      const raw = {
        id: '1',
        title: 'Job Title',
        description: 'Job Description',
        status: 'Available',
        pickupLocation: 'Location A',
        dropoffLocation: 'Location B',
        scheduledDate: '2025-09-19T10:00:00Z',
        assignedVehicleId: 'v1',
        assignedVehicle: {
          id: 'v1',
          make: 'Ford',
          model: 'Transit',
          registrationNumber: 'ABC123',
          assignedJobs: []
        },
        assignedDriverId: 'u1',
        assignedDriver: {
          id: 'u1',
          fullName: 'Jane Doe',
          email: 'jane@example.com'
        }
      };

      const result: TransportJob = service.toTransportJob(raw);

      expect(result.id).toBe(raw.id);
      expect(result.title).toBe(raw.title);
      expect(result.assignedVehicle?.make).toBe('Ford');
      expect(result.assignedDriver?.fullName).toBe('Jane Doe');
    });
  });

  describe('toVehicle', () => {
    it('should map raw data to Vehicle', () => {
      const raw = {
        id: 'v1',
        make: 'Toyota',
        model: 'Hiace',
        registrationNumber: 'XYZ789',
        assignedJobs: [
          {
            id: 'j1',
            title: 'Delivery',
            description: 'Deliver goods',
            status: 'Available',
            pickupLocation: 'Depot',
            dropoffLocation: 'Store',
            scheduledDate: '2025-09-20T09:00:00Z'
          }
        ]
      };

      const result: Vehicle = service.toVehicle(raw);

      expect(result.id).toBe('v1');
      expect(result.make).toBe('Toyota');
      expect(result.assignedJobs?.length).toBe(1);
      expect(result.assignedJobs?.[0].title).toBe('Delivery');
    });

    it('should return undefined for assignedJobs if not an array', () => {
      const raw = {
        id: 'v2',
        make: 'Nissan',
        model: 'NV200',
        registrationNumber: 'LMN456',
        assignedJobs: null
      };

      const result = service.toVehicle(raw);
      expect(result.assignedJobs).toBeUndefined();
    });
  });

  describe('toMinimalUser', () => {
    it('should map raw data to MinimalUser', () => {
      const raw = {
        id: 'u2',
        fullName: 'John Smith',
        email: 'john@example.com'
      };

      const result: MinimalUser = service.toMinimalUser(raw);

      expect(result.id).toBe('u2');
      expect(result.fullName).toBe('John Smith');
      expect(result.email).toBe('john@example.com');
    });
  });
});
