export interface Branch {
  id: string;
  name: string;
  address: string;
  phoneNumber?: string;
  isActive: boolean;
}

export interface Facility {
  id: string;
  branchId: string;
  name: string;
  type: string; // Gym, Turf, Court
  capacity: number;
  pricePerHour: number;
  isActive: boolean;
}
