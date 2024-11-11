import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ActionsRoutingModule } from './actions-routing.module';
import {
  ClickedComponent
} from "./clicked/clicked.component";


@NgModule({
  declarations: [
    ClickedComponent
  ],
  imports: [
    CommonModule,
    ActionsRoutingModule
  ]
})
export class ActionsModule { }
