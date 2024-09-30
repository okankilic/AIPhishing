import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthRoutingModule } from './auth-routing.module';
import { AuthLoginComponent } from './login/auth-login.component';
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


@NgModule({
  declarations: [
    AuthLoginComponent
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
