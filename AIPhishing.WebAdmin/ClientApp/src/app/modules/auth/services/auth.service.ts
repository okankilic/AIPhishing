import {
  Injectable
} from '@angular/core';
import {
  HttpClient
} from "@angular/common/http";
import {
  AuthLoginRequest
} from "../models/auth-login-request";
import {
  AuthLoginResponse
} from "../models/auth-login-response";
import {
  BaseApiService
} from "../../../shared/services/base-api.service";
import {
  AuthUpdatePasswordRequest
} from "../models/auth-update-password-request";

@Injectable({
  providedIn: 'root'
})
export class AuthService extends BaseApiService {

  _baseUrl: string = 'api/auth';

  constructor(httpClient: HttpClient) {
    super(httpClient);
  }

  public login(request: AuthLoginRequest) {
    return this.doPost<AuthLoginResponse>(`${this._baseUrl}/login`, request);
  }

  public updatePassword(request: AuthUpdatePasswordRequest) {
    return this.doPut(`${this._baseUrl}/update-password`, request);
  }
}
