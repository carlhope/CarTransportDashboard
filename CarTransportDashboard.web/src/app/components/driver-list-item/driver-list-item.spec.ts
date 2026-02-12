import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DriverListItem } from './driver-list-item';

describe('DriverListItem', () => {
  let component: DriverListItem;
  let fixture: ComponentFixture<DriverListItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DriverListItem]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DriverListItem);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
