import { TestBed } from '@angular/core/testing';
import {
  HttpClientTestingModule,
  HttpTestingController
} from '@angular/common/http/testing';

import { SalespersonService } from './salesperson.service';

describe('SalespersonService', () => {
  let service: SalespersonService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    service = TestBed.inject(SalespersonService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should get salespeople', () => {
    const salespeople = [
      { id: 1, name: 'Alice Jensen' },
      { id: 2, name: 'Bob Hansen' }
    ];

    service.getSalespeople().subscribe(result => {
      expect(result).toEqual(salespeople);
    });

    const request = httpMock.expectOne('/api/salespersons');

    expect(request.request.method).toBe('GET');

    request.flush(salespeople);
  });
});
