import { TestBed } from '@angular/core/testing';

import { SessionReady } from './session-ready';

describe('SessionReady', () => {
  let service: SessionReady;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SessionReady);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
