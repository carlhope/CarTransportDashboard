import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobTabs } from './job-tabs';

describe('JobTabs', () => {
  let component: JobTabs;
  let fixture: ComponentFixture<JobTabs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobTabs]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JobTabs);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
