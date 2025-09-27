import {TestBed} from '@angular/core/testing';
import {HttpClientTestingModule, HttpTestingController} from '@angular/common/http/testing';
import {VehicleService} from './vehicle';
import {ModelMapperService} from '../model-mapper/model-mapper';
import {Vehicle} from '../../models/vehicle';
import {fuelType} from '../../models/fuel-type';

describe('VehicleService', () => {
  let service: VehicleService;
  let httpMock: HttpTestingController;
  let mapperSpy: jasmine.SpyObj<ModelMapperService>;

  beforeEach(() => {
    mapperSpy = jasmine.createSpyObj('ModelMapperService', ['toVehicle']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        VehicleService,
        { provide: ModelMapperService, useValue: mapperSpy }
      ]
    });

    service = TestBed.inject(VehicleService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  describe('getVehicles', () => {
    it('should fetch and map vehicles', () => {
      // Arrange
      const mockRawVehicles = [{ id: 'v1', make: 'Ford' }];
      const mappedVehicle: Vehicle = {
        id: 'v1',
        make: 'Ford',
        model: 'Transit',
        registrationNumber: 'ABC123',
        assignedJobs: [],
        fuelType: fuelType.Diesel
      };
      mapperSpy.toVehicle.and.returnValue(mappedVehicle);

      // Act
      service.getVehicles().subscribe(vehicles => {
        // Assert
        expect(vehicles.length).toBe(1);
        expect(vehicles[0]).toEqual(mappedVehicle);
        expect(mapperSpy.toVehicle).toHaveBeenCalledWith(mockRawVehicles[0]);
      });
      const req = httpMock.expectOne('https://localhost:7286/api/vehicle');
      expect(req.request.method).toBe('GET');
      req.flush(mockRawVehicles);
    });
  });

  describe('getVehicleById', () => {
    it('should fetch and map a single vehicle by ID', () => {
      // Arrange
      const mockRawVehicle = { id: 'v2', make: 'Toyota' };
      const mappedVehicle: Vehicle = {
        id: 'v2',
        make: 'Toyota',
        model: 'Hiace',
        registrationNumber: 'XYZ789',
        assignedJobs: [],
        fuelType: fuelType.Diesel
      };
      mapperSpy.toVehicle.and.returnValue(mappedVehicle);

      // Act
      service.getVehicleById('v2').subscribe(vehicle => {
        // Assert
        expect(vehicle).toEqual(mappedVehicle);
        expect(mapperSpy.toVehicle).toHaveBeenCalledWith(mockRawVehicle);
      });

      const req = httpMock.expectOne('https://localhost:7286/api/vehicle/v2');
      expect(req.request.method).toBe('GET');
      req.flush(mockRawVehicle);
    });
  });

  describe('create', () => {
    it('should send POST request to create a vehicle', () => {
      const newVehicle: Vehicle = {
        id: 'v3',
        make: 'Nissan',
        model: 'NV200',
        registrationNumber: 'LMN456',
        assignedJobs: [],
        fuelType: fuelType.Diesel
      };

      service.create(newVehicle).subscribe(response => {
        expect(response).toEqual(newVehicle);
      });

      const req = httpMock.expectOne('https://localhost:7286/api/vehicle');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(newVehicle);
      req.flush(newVehicle);
    });
  });

  describe('update', () => {
    it('should send PUT request to update a vehicle', () => {
      const updatedVehicle: Vehicle = {
        id: 'v4',
        make: 'Renault',
        model: 'Kangoo',
        registrationNumber: 'DEF789',
        assignedJobs: [],
        fuelType: fuelType.Diesel
      };

      service.update('v4', updatedVehicle).subscribe(response => {
        expect(response).toEqual(updatedVehicle);
      });

      const req = httpMock.expectOne('https://localhost:7286/api/vehicle/v4');
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updatedVehicle);
      req.flush(updatedVehicle);
    });
  });

  describe('delete', () => {
    it('should send DELETE request to remove a vehicle', () => {
      service.delete('v5').subscribe(response => {
        expect(response).toBeNull();
      });

      const req = httpMock.expectOne('https://localhost:7286/api/vehicle/v5');
      expect(req.request.method).toBe('DELETE');
      req.flush(null);
    });
  });
});
