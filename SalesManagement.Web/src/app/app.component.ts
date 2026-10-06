import { Component, OnInit } from '@angular/core';

import { District } from './models/district';
import { DistrictDetails } from './models/district-details';
import { DistrictService } from './services/district.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  districts: District[] = [];
  selectedDistrict: DistrictDetails | null = null;
  errorMessage = '';

  constructor(private readonly districtService: DistrictService) {}

  ngOnInit(): void {
    this.loadDistricts();
  }

  selectDistrict(district: District): void {
    this.errorMessage = '';

    this.districtService
      .getDistrictDetails(district.id)
      .subscribe({
        next: details => {
          this.selectedDistrict = details;
        },
        error: () => {
          this.errorMessage = 'Unable to load district details.';
        }
      });
  }

  private loadDistricts(): void {
    this.districtService
      .getDistricts()
      .subscribe({
        next: districts => {
          this.districts = districts;

          if (districts.length > 0) {
            this.selectDistrict(districts[0]);
          }
        },
        error: () => {
          this.errorMessage = 'Unable to load districts.';
        }
      });
  }
}
