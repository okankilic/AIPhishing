import {
  AfterViewInit,
  Component
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
export class ClickedComponent implements AfterViewInit {

  private _emailId!: string;

  constructor(
    activatedRoute: ActivatedRoute,
    appContextService: AppContextService,
    private _webhookService: WebhookService
  ) {
    appContextService.showImage = true;
    this._emailId = activatedRoute.snapshot.params.emailId;
  }

  ngAfterViewInit() {
    setTimeout(() => {
      this._webhookService.clicked(this._emailId)
        .subscribe(_ => {

        });
    }, 2000)
  }
}
