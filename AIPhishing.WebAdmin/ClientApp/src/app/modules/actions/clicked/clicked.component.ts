import {
  AfterViewInit,
  Component,
  OnDestroy
} from '@angular/core';
import {
  ActivatedRoute
} from "@angular/router";
import {
  WebhookService
} from "../services/webhook.service";
import {
  AppContextService
} from "../../../core/services/app-context.service";

@Component({
  selector: 'app-clicked',
  templateUrl: './clicked.component.html',
  styleUrls: ['./clicked.component.css']
})
export class ClickedComponent implements AfterViewInit, OnDestroy {

  private _emailId!: string;
  private _timeoutId = -1;

  constructor(
    activatedRoute: ActivatedRoute,
    appContextService: AppContextService,
    private _webhookService: WebhookService
  ) {
    appContextService.showImage = true;
    this._emailId = activatedRoute.snapshot.params.emailId;
  }

  ngAfterViewInit() {
    this.setClickTimeout();
  }

  private setClickTimeout() {
    this._timeoutId = window?.setTimeout(() => {
      this._webhookService.clicked(this._emailId)
        .subscribe(_ => {

        });
    }, 2000)
  }

  ngOnDestroy() {
    if (this._timeoutId > -1) {
      window?.clearTimeout(this._timeoutId);
    }
  }
}
