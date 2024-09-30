import {
  NgModule
} from '@angular/core';
import {
  CommonModule
} from '@angular/common';

import {
  ClientsRoutingModule
} from './clients-routing.module';
import {
  ClientListComponent
} from './list/client-list.component';
import {
  ClientCreateComponent
} from './create/client-create.component';
import {
  MatTableModule
} from "@angular/material/table";
import {
  MatPaginatorModule
} from "@angular/material/paginator";
import {
  MatButtonModule
} from "@angular/material/button";
import {
  ReactiveFormsModule
} from "@angular/forms";
import {
  MatInputModule
} from "@angular/material/input";
import { ClientDetailsComponent } from './details/client-details.component';
import {
  MatListModule
} from "@angular/material/list";
import {
  MatCardModule
} from "@angular/material/card";


@NgModule({
  declarations: [
    ClientListComponent,
    ClientCreateComponent,
    ClientDetailsComponent
  ],
  imports: [
    CommonModule,
    ClientsRoutingModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatListModule,
    MatCardModule
  ]
})
export class ClientsModule { }
