import { Component, OnInit } from '@angular/core';

import { District } from './models/district';
import { DistrictDetails } from './models/district-details';
import { DistrictService } from './services/district.service';
import { Salesperson } from './models/salesperson';
import { SalespersonRole } from './models/salesperson-role';
import { SalespersonService } from './services/salesperson.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
  districts: District[] = [];
  selectedDistrict: DistrictDetails | null = null;

  salespeople: Salesperson[] = [];
  selectedSalespersonId: number | null = null;
  selectedRole: SalespersonRole = 'Secondary';

  errorMessage = '';

  constructor(
    private readonly districtService: DistrictService,
    private readonly salespersonService: SalespersonService
  ) {}

  ngOnInit(): void {
    this.loadDistricts();
    this.loadSalespeople();
  }

  selectDistrict(district: District): void {
    this.errorMessage = '';
    this.loadDistrictDetails(district.id);
  }

  get availableSalespeople(): Salesperson[] {
    if (!this.selectedDistrict) {
      return [];
    }

    const assignedIds = new Set(
      this.selectedDistrict.salespersons.map(salesperson => salesperson.id)
    );

    return this.salespeople.filter(
      salesperson => !assignedIds.has(salesperson.id)
    );
  }

  assignSalesperson(): void {
    if (!this.selectedDistrict || this.selectedSalespersonId === null) {
      return;
    }

    this.errorMessage = '';

    this.districtService
      .assignSalesperson(
        this.selectedDistrict.id,
        this.selectedSalespersonId,
        { role: this.selectedRole }
      )
      .subscribe({
        next: () => {
          this.loadDistrictDetails(this.selectedDistrict!.id);
          this.selectedSalespersonId = null;
        },
        error: () => {
          this.errorMessage = 'Unable to assign salesperson.';
        }
      });
  }

  removeSalesperson(salespersonId: number): void {
    if (!this.selectedDistrict) {
      return;
    }

    this.errorMessage = '';

    this.districtService
      .removeSalesperson(this.selectedDistrict.id, salespersonId)
      .subscribe({
        next: () => {
          this.loadDistrictDetails(this.selectedDistrict!.id);
        },
        error: error => {
          if (error.status === 409) {
            this.errorMessage =
              error.error?.message ?? 'The district must have a primary salesperson.';
          } else {
            this.errorMessage = 'Unable to remove salesperson.';
          }
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

  private loadSalespeople(): void {
    this.salespersonService
      .getSalespeople()
      .subscribe({
        next: salespeople => {
          this.salespeople = salespeople;
        },
        error: () => {
          this.errorMessage = 'Unable to load salespeople.';
        }
      });
  }

  private loadDistrictDetails(districtId: number): void {
    this.districtService
      .getDistrictDetails(districtId)
      .subscribe({
        next: details => {
          this.selectedDistrict = details;
        },
        error: () => {
          this.errorMessage = 'Unable to load district details.';
        }
      });
  }
}
