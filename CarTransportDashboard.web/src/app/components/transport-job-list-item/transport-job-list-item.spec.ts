import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransportJobListItem } from './transport-job-list-item';

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
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
