import { Injectable } from '@angular/core';
import{TransportJob, JobStatus} from '../models/transport-job';
import {HttpClient} from '@angular/common/http';
import {map, Observable} from 'rxjs';
import {Vehicle} from '../models/vehicle';
import {MinimalUser} from '../models/minimal-user';
import {ModelMapperService} from './model-mapper';

@Injectable({
  providedIn: 'root'
})
export class TransportJobService {
  private apiUrl = 'http://localhost:5176/api/transportjobs';

  constructor(private http: HttpClient, private mapper: ModelMapperService) {}

  getJobs(): Observable<TransportJob[]> {
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(jobs => jobs.map(job => this.mapper.toTransportJob(job)))
    );
  }
  getAvailableJobs(): Observable<TransportJob[]> {
  return this.http.get<TransportJob[]>(`${this.apiUrl}/available`);
}
  getJobById(id: string): Observable<TransportJob> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.get<any>(url).pipe(
      map(data => this.mapper.toTransportJob(data))
    );
  }
  create(job: TransportJob): Observable<TransportJob> {
    return this.http.post<TransportJob>(this.apiUrl, job);
  }

  update(id: string, job: TransportJob): Observable<TransportJob> {
    return this.http.put<TransportJob>(`${this.apiUrl}/${id}`, job);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
  updateJobStatus(id: string, status: JobStatus): Observable<TransportJob> {
  return this.http.put<TransportJob>(`${this.apiUrl}/status/${id}`, status);
}

acceptJob(id: string, driverId: string): Observable<TransportJob> {
  return this.http.put<TransportJob>(`${this.apiUrl}/accept/${id}`, driverId);
}

assignVehicle(id: string, vehicleId: string): Observable<TransportJob> {
  return this.http.put<TransportJob>(`${this.apiUrl}/assign-vehicle/${id}`, vehicleId);
}

assignDriver(id: string, driverId: string): Observable<TransportJob> {
  return this.http.put<TransportJob>(`${this.apiUrl}/assign-driver/${id}`, driverId);
}

}
