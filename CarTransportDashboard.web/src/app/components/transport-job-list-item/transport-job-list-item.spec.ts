import {ComponentFixture, TestBed} from '@angular/core/testing';

import {TransportJobListItem} from './transport-job-list-item';
import {JobStatus} from '../../models/job-status';
import {fuelType} from '../../models/fuel-type';
import {Address} from '../../models/Address';

const mockPickupLocation: Address = {
  companyName: 'Acme Supplies Ltd',
  addressLine1: 'Unit 4, Acme Business Park',
  addressLine2: 'Warehouse Entrance',
  locality: 'Stoke-on-Trent',
  postalCode: 'ST1 1AA',
  country: 'GB',
  lat: 53.0027,
  lng: -2.1794,
  formatted: 'Acme Supplies Ltd, Unit 4, Acme Business Park, Stoke-on-Trent ST1 1AA, UK'
};

const mockDropoffLocation: Address = {
  companyName: 'Derby Distribution Hub',
  addressLine1: '456 Industrial Estate',
  addressLine2: 'Loading Bay 3',
  locality: 'Derby',
  postalCode: 'DE1 2BB',
  country: 'GB',
  lat: 52.9225,
  lng: -1.4746,
  formatted: 'Derby Distribution Hub, 456 Industrial Estate, Derby DE1 2BB, UK'
};

describe('TransportJobListItem', () => {
  let component: TransportJobListItem;
  let fixture: ComponentFixture<TransportJobListItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransportJobListItem]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TransportJobListItem);
    component = fixture.componentInstance;
    component.job = {
      id: '1',
      title: 'Test Job',
      description: 'Test Description',
      pickupLocation: mockPickupLocation,
      dropoffLocation: mockDropoffLocation,
      scheduledDate: new Date().toISOString(),
      status: JobStatus.Available,
      assignedVehicleId: '123',
      assignedVehicle: {
        make: 'Toyota',
        model: 'Camry',
        registrationNumber: 'ABC123',
        fuelType: fuelType.Diesel
      }
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
