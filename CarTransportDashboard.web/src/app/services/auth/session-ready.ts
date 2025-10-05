import { Injectable } from '@angular/core';
import {BehaviorSubject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SessionReadyService {
  private readySubject = new BehaviorSubject<boolean>(false);
  sessionReady$ = this.readySubject.asObservable();

  markReady() {
    this.readySubject.next(true);
  }
}

