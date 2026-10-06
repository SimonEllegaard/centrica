import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { District } from '../models/district';
import { DistrictDetails } from '../models/district-details';
import { AssignSalespersonRequest } from '../models/assign-salesperson-request';

@Injectable({
  providedIn: 'root'
})
export class DistrictService {
  private readonly apiUrl = '/api/districts';

  constructor(private readonly http: HttpClient) {}

  getDistricts(): Observable<District[]> {
    return this.http.get<District[]>(this.apiUrl);
  }

  getDistrictDetails(districtId: number): Observable<DistrictDetails> {
    return this.http.get<DistrictDetails>(
      `${this.apiUrl}/${districtId}`
    );
  }

  assignSalesperson(
    districtId: number,
    salespersonId: number,
    request: AssignSalespersonRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${districtId}/salespersons/${salespersonId}`,
      request
    );
  }

  removeSalesperson(
    districtId: number,
    salespersonId: number
  ): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${districtId}/salespersons/${salespersonId}`
    );
  }
}
