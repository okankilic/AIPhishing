export interface AuthLoginResponse {
  apiKey: string;
  tokenExpiry: string | Date;
  user: AuthUserResponse;
}

export interface AuthUserResponse {
  email: string;
  clientId: string | null;
}
