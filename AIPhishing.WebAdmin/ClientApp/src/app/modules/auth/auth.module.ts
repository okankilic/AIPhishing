import {
  NgModule
} from '@angular/core';
import {
  CommonModule
} from '@angular/common';

import {
  AuthRoutingModule
} from './auth-routing.module';
import {
  AuthLoginComponent
} from './login/auth-login.component';
import {
  MatInputModule
} from "@angular/material/input";
import {
  MatButtonModule
} from "@angular/material/button";
import {
  FormsModule,
  ReactiveFormsModule
} from "@angular/forms";
import { AuthAccountComponent } from './account/auth-account.component';


@NgModule({
  declarations: [
    AuthLoginComponent,
    AuthAccountComponent
  ],
  imports: [
    CommonModule,
    AuthRoutingModule,
    MatInputModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class AuthModule { }
