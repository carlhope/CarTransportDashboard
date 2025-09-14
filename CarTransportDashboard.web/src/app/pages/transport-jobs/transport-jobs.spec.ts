import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransportJobs } from './transport-jobs';

describe('TransportJobs', () => {
  let component: TransportJobs;
  let fixture: ComponentFixture<TransportJobs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransportJobs]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TransportJobs);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
