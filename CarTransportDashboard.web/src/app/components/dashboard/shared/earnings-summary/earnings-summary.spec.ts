import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EarningsSummary } from './earnings-summary';

describe('EarningsSummary', () => {
  let component: EarningsSummary;
  let fixture: ComponentFixture<EarningsSummary>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EarningsSummary]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EarningsSummary);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
