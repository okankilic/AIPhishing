import {
  BrowserModule
} from '@angular/platform-browser';
import {
  ErrorHandler,
  NgModule
} from '@angular/core';
import {
  FormsModule
} from '@angular/forms';
import {
  HTTP_INTERCEPTORS,
  HttpClientModule
} from '@angular/common/http';
import {
  RouterModule
} from '@angular/router';

import {
  AppComponent
} from './app.component';
import {
  NavMenuComponent
} from './nav-menu/nav-menu.component';
import {
  BrowserAnimationsModule
} from '@angular/platform-browser/animations';
import {
  JwtInterceptor
} from "./core/interceptors/jwt.interceptor";
import {
  AuthGuard
} from "./core/guards/auth.guard";
import {
  MatButtonModule
} from "@angular/material/button";
import {
  GlobalErrorHandler
} from "./core/handlers/global-error-handler";
import {
  MatSnackBarModule
} from "@angular/material/snack-bar";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      {
        path: 'clients',
        loadChildren: () => import('./modules/clients/clients.module').then(m => m.ClientsModule),
        canActivate: [
          AuthGuard
        ]
      },
      {
        path: 'login',
        loadChildren: () => import('./modules/auth/auth.module').then(m => m.AuthModule)
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'clients'
      }
    ]),
    BrowserAnimationsModule,
    MatButtonModule,
    MatSnackBarModule
  ],
  providers: [
    {
      provide: ErrorHandler,
      useClass: GlobalErrorHandler
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }