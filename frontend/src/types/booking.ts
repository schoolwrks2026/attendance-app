export interface Booking {
  id: string;
  facilityId: string;
  facilityName: string;
  facilityType: string;
  userId: string;
  memberName: string;
  bookingDate: string;
  startTime: string; // "HH:mm"
  endTime: string;   // "HH:mm"
  status: string;    // "Confirmed" | "Cancelled"
  notes?: string;
}
