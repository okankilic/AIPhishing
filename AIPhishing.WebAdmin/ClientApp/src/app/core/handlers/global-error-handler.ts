import {
  HttpErrorResponse
} from '@angular/common/http';
import {
  ErrorHandler,
  Injectable
} from '@angular/core';
import {
  AppContextService
} from "../services/app-context.service";
import {
  SnackbarService
} from "../services/snackbar.service";

@Injectable({
  providedIn: 'root'
})
export class GlobalErrorHandler implements ErrorHandler {

  constructor(
    private readonly _appContextService: AppContextService,
    private readonly snackbarService: SnackbarService) {
  }

  handleError(error: Error | HttpErrorResponse) {
    if (error instanceof HttpErrorResponse) {
      const httpErrorResponse = error as HttpErrorResponse;
      if (httpErrorResponse.status === 401) {
        this.snackbarService.show('error', `Your session expired. You will be redirected to login.`);
        setTimeout(() => {
          this._appContextService.logout();
          location.reload();
        }, 1000);
      } else {
        this.snackbarService.show('Error', httpErrorResponse.error.errorMessage);
      }
    } else {
      console.error(error)
    }
  }
}
