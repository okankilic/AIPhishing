import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {
  ClickedComponent
} from "./clicked/clicked.component";

const routes: Routes = [
  {
    path: ':emailId',
    component: ClickedComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ActionsRoutingModule { }
