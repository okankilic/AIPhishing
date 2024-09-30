import {
  NgModule
} from '@angular/core';
import {
  RouterModule,
  Routes
} from '@angular/router';
import {
  ClientListComponent
} from "./list/client-list.component";
import {
  ClientCreateComponent
} from "./create/client-create.component";
import {
  ClientDetailsComponent
} from "./details/client-details.component";

const routes: Routes = [
  {
    path: '',
    component: ClientListComponent
  },
  {
    path: 'create',
    component: ClientCreateComponent
  },
  {
    path: ':id',
    component: ClientDetailsComponent,
    // canActivate: [
    //   AuthGuard
    // ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ClientsRoutingModule { }
