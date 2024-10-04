import { Injectable } from '@angular/core';
import {
  AuthLoginResponse,
  AuthUserResponse
} from "../../modules/auth/models/auth-login-response";
import {
  BehaviorSubject
} from "rxjs";
import {
  NavigationExtras,
  Router
} from "@angular/router";

export interface AppState {
  apiKey: string | null,
  tokenExpiry: string | Date | null,
  user: AuthUserResponse | null
}

@Injectable({
  providedIn: 'root'
})
export class AppContextService {

  private readonly _storageKey = 'ai_phishing_auth';

  private readonly _localStorageItem = localStorage.getItem(this._storageKey);

  private _loginResponse: AuthLoginResponse | null = this._localStorageItem
    ? JSON.parse(this._localStorageItem)
    : null;

  private _state = new BehaviorSubject<AppState>({
    apiKey: this._loginResponse?.apiKey ?? null,
    tokenExpiry: this._loginResponse?.tokenExpiry ?? null,
    user: this._loginResponse?.user ?? null
  });

  get state() {
    return this._state;
  }

  constructor(
    private readonly _router: Router) {

  }

  private setState(state: Partial<AppState>) {
    const newState = Object.assign(this.state.value, state);
    this._state.next(newState);
  }

  setLoginResponse(result: AuthLoginResponse, returnUrl: string) {
    localStorage.setItem(this._storageKey, JSON.stringify(result));
    this.setState({
      apiKey: result.apiKey,
      tokenExpiry: result.tokenExpiry,
      user: result.user
    })
    this.navigate(returnUrl ?? '/');
  }

  navigate(url: string, extras?: NavigationExtras) {
    this._router.navigate([url], extras);
  }

  logout(returnUrl: string = '/') {
    localStorage.removeItem(this._storageKey);
    this.setState({
      apiKey: null,
      tokenExpiry: null,
      user: null
    })
    this.navigate('/login', {
      queryParams: {
        returnUrl: returnUrl ?? '/'
      }
    })
  }
}
