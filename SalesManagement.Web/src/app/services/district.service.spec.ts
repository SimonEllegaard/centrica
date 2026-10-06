import { TestBed } from '@angular/core/testing';
import {
  HttpClientTestingModule,
  HttpTestingController
} from '@angular/common/http/testing';

import { DistrictService } from './district.service';

describe('DistrictService', () => {
  let service: DistrictService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    service = TestBed.inject(DistrictService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should get districts', () => {
    const districts = [
      { id: 1, name: 'North Denmark' },
      { id: 2, name: 'Southern Denmark' }
    ];

    service.getDistricts().subscribe(result => {
      expect(result).toEqual(districts);
    });

    const request = httpMock.expectOne('/api/districts');

    expect(request.request.method).toBe('GET');

    request.flush(districts);
  });

  it('should get district details', () => {
    const district = {
      id: 1,
      name: 'North Denmark',
      salespersons: [],
      stores: []
    };

    service.getDistrictDetails(1).subscribe(result => {
      expect(result).toEqual(district);
    });

    const request = httpMock.expectOne('/api/districts/1');

    expect(request.request.method).toBe('GET');

    request.flush(district);
  });

  it('should assign a salesperson', () => {
    const requestBody = { role: 'Primary' as const };

    service.assignSalesperson(1, 2, requestBody).subscribe();

    const request = httpMock.expectOne(
      '/api/districts/1/salespersons/2'
    );

    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(requestBody);

    request.flush(null);
  });

  it('should remove a salesperson', () => {
    service.removeSalesperson(1, 2).subscribe();

    const request = httpMock.expectOne(
      '/api/districts/1/salespersons/2'
    );

    expect(request.request.method).toBe('DELETE');

    request.flush(null);
  });
});
