import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';

import { AuthService } from './auth';
import {Login} from '../../pages/auth/login/login';

describe('Auth', () => {
  let service: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient()
      ]
    });
    service = TestBed.inject(AuthService);
  });


  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

