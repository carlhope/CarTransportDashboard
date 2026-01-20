import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobMap } from './job-map';

describe('JobMap', () => {
  let component: JobMap;
  let fixture: ComponentFixture<JobMap>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobMap]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JobMap);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
