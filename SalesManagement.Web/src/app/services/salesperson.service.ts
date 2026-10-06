import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Salesperson } from '../models/salesperson';

@Injectable({
  providedIn: 'root'
})
export class SalespersonService {
  private readonly apiUrl = '/api/salespersons';

  constructor(private readonly http: HttpClient) {}

  getSalespeople(): Observable<Salesperson[]> {
    return this.http.get<Salesperson[]>(this.apiUrl);
  }
}
