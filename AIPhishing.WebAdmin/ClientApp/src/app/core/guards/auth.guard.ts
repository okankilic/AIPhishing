import {
  Injectable
} from "@angular/core";
import {
  ActivatedRouteSnapshot,
  CanActivate,
  RouterStateSnapshot
} from "@angular/router";
import {
  AppContextService
} from "../services/app-context.service";
import {
  SnackbarService
} from "../services/snackbar.service";

@Injectable({providedIn: 'root'})
export class AuthGuard implements CanActivate {
  constructor(
    private _appContextService: AppContextService,
    private _snackbarService: SnackbarService
  ) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (this._appContextService.state?.value?.user) {
      if (this._appContextService.state?.value?.tokenExpiry) {
        const tokenExpiry = new Date(this._appContextService.state.value.tokenExpiry);
        const now = new Date();
        if (tokenExpiry < now) {
          this._snackbarService.show('error', 'Your session has been expired. You should login again.');
          this._appContextService.logout(state.url);
          return false;
        }
      }
      return true;
    }
    this._appContextService.logout(state.url);
    return false;
  }
}
