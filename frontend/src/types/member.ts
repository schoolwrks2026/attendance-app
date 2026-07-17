export interface MemberProfile {
  id: string;
  userId: string;
  fullName: string;
  email: string;
  membershipType: string; // None, Monthly, Annual, Premium
  membershipStartDate: string;
  membershipEndDate: string;
  isMembershipActive: boolean;
  medicalNotes?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
}
