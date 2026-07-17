export interface User {
  id: string;
  username: string;
  email: string;
  role: string;
  fullName: string;
  phoneNumber?: string;
}

export interface AuthResponse {
  user: User;
  token: string;
}
