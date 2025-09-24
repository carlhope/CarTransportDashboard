import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, map } from 'rxjs';
import { Vehicle } from '../../models/vehicle';
import { ModelMapperService } from '../model-mapper/model-mapper';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  private apiUrl = 'https://localhost:7286/api/vehicle';
  //private apiUrl = 'http://localhost:5176/api/vehicle';

  constructor(private http: HttpClient, private mapper: ModelMapperService) {}

  getVehicles(): Observable<Vehicle[]> {
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(vehicles => vehicles.map(vehicle => this.mapper.toVehicle(vehicle)))
    );
  }

  getVehicleById(id: string): Observable<Vehicle> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.get<any>(url).pipe(
      map(data => this.mapper.toVehicle(data))
    );
  }
  create(vehicle: Vehicle): Observable<Vehicle> {
    return this.http.post<Vehicle>(this.apiUrl, vehicle);
  }

  update(id: string, vehicle: Vehicle): Observable<Vehicle> {
    return this.http.put<Vehicle>(`${this.apiUrl}/${id}`, vehicle);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
  getVehicleByRegistration(reg: string): Observable<Vehicle> {
    const normalizedReg = reg.replace(/\s+/g, '').toUpperCase();
    //debugger;
    console.log(normalizedReg);
    console.log(this.apiUrl+"/registration/"+normalizedReg);
    return this.http.get<Vehicle>(`${this.apiUrl}/registration/${normalizedReg}`);

  }
}
