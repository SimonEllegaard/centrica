import { DistrictSalesperson } from './district-salesperson';
import { Store } from './store';

export interface DistrictDetails {
  id: number;
  name: string;
  salespersons: DistrictSalesperson[];
  stores: Store[];
}
