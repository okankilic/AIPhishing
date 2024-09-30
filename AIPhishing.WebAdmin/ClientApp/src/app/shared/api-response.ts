export interface ApiResponse<T> {
  errorCode: number;
  errorMessage: string | null;
  result: T;
}
