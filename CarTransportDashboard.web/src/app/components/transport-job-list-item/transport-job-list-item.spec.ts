import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransportJobListItem } from './transport-job-list-item';
import {JobStatus} from '../../models/job-status';

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
      pickupLocation: 'A',
      dropoffLocation: 'B',
      scheduledDate: new Date().toISOString(),
      status: JobStatus.Available,
      assignedVehicleId: '123',
      assignedVehicle: {
        make: 'Toyota',
        model: 'Camry',
        registrationNumber: 'ABC123'
      }
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
