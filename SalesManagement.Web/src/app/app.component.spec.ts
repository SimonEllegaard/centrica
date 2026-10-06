import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';

import { AppComponent } from './app.component';
import { DistrictService } from './services/district.service';
import { SalespersonService } from './services/salesperson.service';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let districtService: jasmine.SpyObj<DistrictService>;
  let salespersonService: jasmine.SpyObj<SalespersonService>;

  beforeEach(async () => {
    districtService = jasmine.createSpyObj('DistrictService', [
      'getDistricts',
      'getDistrictDetails',
      'assignSalesperson',
      'removeSalesperson'
    ]);

    salespersonService = jasmine.createSpyObj('SalespersonService', [
      'getSalespeople'
    ]);

    districtService.getDistricts.and.returnValue(of([
      { id: 1, name: 'North Denmark' },
      { id: 2, name: 'Southern Denmark' }
    ]));

    districtService.getDistrictDetails.and.returnValue(of({
      id: 1,
      name: 'North Denmark',
      salespersons: [
        {
          id: 1,
          name: 'Alice Jensen',
          role: 'Primary'
        },
        {
          id: 2,
          name: 'Bob Hansen',
          role: 'Secondary'
        }
      ],
      stores: [
        { id: 1, name: 'Aalborg' }
      ]
    }));

    salespersonService.getSalespeople.and.returnValue(of([
      { id: 1, name: 'Alice Jensen' },
      { id: 2, name: 'Bob Hansen' },
      { id: 3, name: 'Charlie Nielsen' }
    ]));

    await TestBed.configureTestingModule({
      declarations: [AppComponent],
      imports: [FormsModule],
      providers: [
        {
          provide: DistrictService,
          useValue: districtService
        },
        {
          provide: SalespersonService,
          useValue: salespersonService
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should load districts and select the first district', () => {
    expect(component.districts.length).toBe(2);
    expect(component.selectedDistrict?.id).toBe(1);
    expect(component.selectedDistrict?.name).toBe('North Denmark');
  });

  it('should only show unassigned salespeople as available', () => {
    expect(component.availableSalespeople).toEqual([
      { id: 3, name: 'Charlie Nielsen' }
    ]);
  });

  it('should make a salesperson primary', () => {
    districtService.assignSalesperson.and.returnValue(of(void 0));

    component.makePrimary(2);

    expect(districtService.assignSalesperson).toHaveBeenCalledWith(
      1,
      2,
      { role: 'Primary' }
    );
  });

  it('should assign a salesperson with the selected role', () => {
    districtService.assignSalesperson.and.returnValue(of(void 0));

    component.selectedSalespersonId = 3;
    component.selectedRole = 'Secondary';

    component.assignSalesperson();

    expect(districtService.assignSalesperson).toHaveBeenCalledWith(
      1,
      3,
      { role: 'Secondary' }
    );
  });

  it('should display an error when removing the primary salesperson', () => {
    districtService.removeSalesperson.and.returnValue(
      throwError(() => ({
        status: 409,
        error: {
          message: 'The district must have a primary salesperson.'
        }
      }))
    );

    spyOn(window, 'confirm').and.returnValue(true);

    component.removeSalesperson(1);

    expect(component.errorMessage)
      .toBe('Unable to remove salesperson.');
  });
});
