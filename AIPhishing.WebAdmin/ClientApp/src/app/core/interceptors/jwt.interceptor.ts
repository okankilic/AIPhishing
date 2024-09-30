import {
  HttpEvent,
  HttpEventType,
  HttpHandler,
  HttpInterceptor,
  HttpRequest
} from '@angular/common/http';
import {
  Injectable
} from '@angular/core';
import {
  catchError,
  delay,
  finalize,
  map,
  Observable,
  tap,
  throwError
} from 'rxjs';
import {
  AppContextService
} from "../services/app-context.service";
import {
  AuthService
} from "../../modules/auth/services/auth.service";

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(
    private _appContextService: AppContextService,
    // private _loadingIndicatorService: LoadingIndicatorService,
    private _authService: AuthService
  ) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    const applicationState = this._appContextService.state?.value;

    if (applicationState?.apiKey
      && request.url.indexOf('amazonaws') < 0
      && request.url.indexOf('googleapis') < 0
    ) {
      request = request.clone({
        setHeaders: {
          'X-API-Key': applicationState?.apiKey
        }
      });
    }

    if (request.url.indexOf('amazonaws') >= 0) {
      console.log(`amazon: ${request.url}`)
    }

    return next.handle(request)
      .pipe(
        delay(0),
        tap(() => {
          // this._loadingIndicatorService.isLoading = true;
        }),
        map((event: any) => {

          if (event.type === HttpEventType.UploadProgress) {
            // this._loadingIndicatorService.mode = 'determinate';
            // this._loadingIndicatorService.progress = Math.round((100 / event.total) * event.loaded);
          }

          if (event.type === HttpEventType.Response) {
            // this._loadingIndicatorService.mode = 'indeterminate';
            // this._loadingIndicatorService.progress = null;
          }

          return event;
        }),
        finalize(() => {
          // this._loadingIndicatorService.isLoading = false;
        }),
        catchError(err => {
          // if (err.status === 401 && applicationState?.refreshToken) {
          //   return this.refreshToken(applicationState.refreshToken, request, next);
          // }
          return throwError(err);
        })
      );
  }

  // private refreshToken(refreshToken: string, request: HttpRequest<any>, next: HttpHandler) {
  //   return this._authenticationService
  //     .refreshToken({
  //       refreshToken: refreshToken
  //     })
  //     .pipe(
  //       switchMap(refreshTokenResponse => {
  //         this._applicationContextService.setRefreshTokenResult(refreshTokenResponse);
  //         const newAccessToken = this._applicationContextService.state.value.accessToken;
  //         if (newAccessToken
  //           && request.url.indexOf('amazonaws') < 0
  //           && request.url.indexOf('googleapis') < 0
  //         ) {
  //           request = request.clone({
  //             setHeaders: {
  //               Authorization: `Bearer ${newAccessToken}`
  //             }
  //           });
  //         }
  //         return next.handle(request);
  //       }),
  //       catchError((error) => {
  //         // Handle refresh token error (e.g., redirect to login page)
  //         console.error('Error handling expired access token:', error);
  //         this._applicationContextService.logout()
  //         return of([]);
  //       })
  //     );
  // }
}
