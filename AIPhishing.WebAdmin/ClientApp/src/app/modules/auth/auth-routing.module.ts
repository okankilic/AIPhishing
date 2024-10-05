import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {
  AuthLoginComponent
} from "./login/auth-login.component";
import {
  AuthAccountComponent
} from "./account/auth-account.component";

const routes: Routes = [
  {
    path: 'login',
    component: AuthLoginComponent
  },
  {
    path: 'account',
    component: AuthAccountComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule { }
