import { Injectable } from '@angular/core';
import{TransportJob} from '../models/transport-job';
import {HttpClient} from '@angular/common/http';
import {map, Observable} from 'rxjs';
import {Vehicle} from '../models/vehicle';
import {MinimalUser} from '../models/minimal-user';
import {ModelMapperService} from './model-mapper';

@Injectable({
  providedIn: 'root'
})
export class TransportJobService {
  private apiUrl = 'https://your-api.com/api/transport-jobs';

  constructor(private http: HttpClient, private mapper: ModelMapperService) {}

  getJobs(): Observable<TransportJob[]> {
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(jobs => jobs.map(job => this.mapper.toTransportJob(job)))
    );
  }
  getJobById(id: string): Observable<TransportJob> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.get<any>(url).pipe(
      map(data => this.mapper.toTransportJob(data))
    );
  }
}
