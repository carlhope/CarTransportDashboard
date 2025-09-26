import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SessionAction } from './session-action';

describe('SessionAction', () => {
  let component: SessionAction;
  let fixture: ComponentFixture<SessionAction>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SessionAction]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SessionAction);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
