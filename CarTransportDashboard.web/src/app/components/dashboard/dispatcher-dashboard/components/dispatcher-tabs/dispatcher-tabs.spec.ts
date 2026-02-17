import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DispatcherTabs } from './dispatcher-tabs';

describe('DispatcherTabs', () => {
  let component: DispatcherTabs;
  let fixture: ComponentFixture<DispatcherTabs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DispatcherTabs]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DispatcherTabs);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
