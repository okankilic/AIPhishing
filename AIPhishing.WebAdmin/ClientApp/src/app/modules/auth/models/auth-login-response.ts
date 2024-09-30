export interface AuthLoginResponse {
  apiKey: string;
  user: AuthUserResponse;
}

export interface AuthUserResponse {
  email: string;
  clientId: string | null;
}
