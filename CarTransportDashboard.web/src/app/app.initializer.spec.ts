import { TestBed } from '@angular/core/testing';
import { initializeSession } from './app.initializer';
import { AuthService } from './services/auth/auth';
import { UserStoreService } from './services/auth/user-store-service';
import {Observable, of, throwError} from 'rxjs';
import {UserModel} from './models/user';
import {Injector, runInInjectionContext} from '@angular/core';

describe('initializeSession', () => {
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let userStoreSpy: jasmine.SpyObj<UserStoreService>;
  let injector: Injector;
  let initFn: () => Observable<UserModel | null>;


  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['refresh']);
    userStoreSpy = jasmine.createSpyObj('UserStoreService', ['setUser', 'clearUser']);

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: UserStoreService, useValue: userStoreSpy }
      ]
    });

    injector = TestBed.inject(Injector);
    initFn = initializeSession(injector);
  });

  it('should set user and mark session ready on success', (done) => {
    const mockUser: UserModel = {
      id: '123e4567-e89b-12d3-a456-426614174000',
      email: 'carl.hope@example.com',
      firstName: 'Carl',
      lastName: 'Hope',
      roles: ['User']
    };

    authServiceSpy.refresh.and.returnValue(of(mockUser));
    runInInjectionContext(injector, () => {
    initFn().subscribe(() => {
      expect(userStoreSpy.setUser).toHaveBeenCalledWith(mockUser);
      done();
    });
    });
  });

  it('should clear user and mark session ready on error', (done) => {
    authServiceSpy.refresh.and.returnValue(throwError(() => new Error('Auth failed')));
    runInInjectionContext(injector, () => {
      initFn().subscribe(() => {
        expect(userStoreSpy.clearUser).toHaveBeenCalled();
        done();
      });
    });
  });
});


