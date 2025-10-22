import { Injectable } from '@angular/core';
import{TransportJob} from '../../models/transport-job';
import { JobStatus } from '../../models/job-status';
import {HttpClient} from '@angular/common/http';
import {map, of, Observable, catchError, throwError} from 'rxjs';
import { tap } from 'rxjs/operators';
import {Vehicle} from '../../models/vehicle';
import {MinimalUser} from '../../models/minimal-user';
import {ModelMapperService} from '../model-mapper/model-mapper';

@Injectable({
  providedIn: 'root'
})
export class TransportJobService {
  private apiUrl = 'https://localhost:7286/api/transportjobs';
  //private apiUrl = 'http://localhost:5176/api/transportjobs';

  constructor(private http: HttpClient, private mapper: ModelMapperService) {}

getJobs(): Observable<TransportJob[]> {
  return this.http.get<any[]>(this.apiUrl, {withCredentials:true}).pipe(
    tap(raw => console.log('Raw API response:', raw)),
    map(jobs => {
      const mapped = jobs.map(job => this.mapper.toTransportJob(job));
      return mapped;
    })
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
  create(payload:  TransportJob ): Observable<TransportJob> {
    return this.http.post<TransportJob>(this.apiUrl, payload);
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

assignVehicle(id: string, vehicleId: string): Observable<TransportJob> {
  return this.http.put<TransportJob>(`${this.apiUrl}/assign-vehicle/${id}`, vehicleId);
}

assignDriver(id: string, driverId: string): Observable<TransportJob> {
  return this.http.put<TransportJob>(`${this.apiUrl}/assign-driver/${id}`, driverId);
}
  getAcceptedJobs(): Observable<TransportJob[]> {
    return this.http.get<any[]>(
      `${this.apiUrl}/myjobs?status=InProgress`,
      { withCredentials: true }
    ).pipe(
      map(jobs => jobs.map(job => this.mapper.toTransportJob(job)))
    );
  }
  getAvailableJobsForDriver(): Observable<TransportJob[]> {
    return this.http.get<any[]>(
      `${this.apiUrl}/myjobs?status=allocated`,
      { withCredentials: true }
    ).pipe(
      map(jobs => jobs.map(job => this.mapper.toTransportJob(job)))
    );
  }
  getCompletedJobs(days: number = 30): Observable<TransportJob[]> {
    const fromDate = new Date();
    fromDate.setDate(fromDate.getDate() - days);
    const isoDate = fromDate.toISOString();

    return this.http.get<any[]>(
      `${this.apiUrl}/myjobs?status=Completed&startDate=${isoDate}`,
      { withCredentials: true }
    ).pipe(
      map(jobs => jobs.map(job => this.mapper.toTransportJob(job)))
    );
  }
  acceptJob(jobId: string): Observable<TransportJob> {
    const url = `${this.apiUrl}/${jobId}/accept`;
    return this.http.post<TransportJob>(url, {}).pipe(
      catchError(err => {
        console.error('Accept job failed:', err);
        return throwError(() => new Error('Could not accept job.'));
      })
    );
  }
  completeJob(jobId: string): Observable<TransportJob> {
    return this.http.post<TransportJob>(`${this.apiUrl}/${jobId}/complete`, {});
  }

  cancelJob(jobId: string): Observable<TransportJob> {
    return this.http.post<TransportJob>(`${this.apiUrl}/${jobId}/cancel`, {});
  }

  unassignJob(jobId: string): Observable<TransportJob> {
    return this.http.post<TransportJob>(`${this.apiUrl}/${jobId}/unassign`, {});
  }





}
