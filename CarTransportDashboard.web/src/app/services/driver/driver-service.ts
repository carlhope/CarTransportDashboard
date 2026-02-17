import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ModelMapperService} from '../model-mapper/model-mapper';
import {Observable} from 'rxjs';
import {TransportJob} from '../../models/transport-job';
import {DriverModel} from '../../models/user';

@Injectable({
  providedIn: 'root'
})
export class DriverService {
  private apiUrl = 'https://localhost:7286/api/driver';

  constructor(private http: HttpClient, private mapper: ModelMapperService) {}

  getAll(): Observable<DriverModel[]> {
    return this.http.get<DriverModel[]>(`${this.apiUrl}/all`);
  }
}
