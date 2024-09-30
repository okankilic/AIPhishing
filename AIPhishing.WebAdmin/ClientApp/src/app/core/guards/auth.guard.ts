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

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(
    private _appContextService: AppContextService
  ) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (this._appContextService.state?.value?.user) {
      return true;
    }
    this._appContextService.logout(state.url);
    return false;
  }
}
