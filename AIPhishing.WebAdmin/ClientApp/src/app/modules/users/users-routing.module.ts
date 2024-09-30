import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {
  UserListComponent
} from "./list/user-list.component";
import {
  UserCreateComponent
} from "./create/user-create.component";

const routes: Routes = [
  {
    path: '',
    component: UserListComponent
  },
  {
    path: 'create',
    component: UserCreateComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsersRoutingModule { }
