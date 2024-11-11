import {
  Injectable
} from '@angular/core';
import {
  BaseApiService
} from "../../../shared/services/base-api.service";
import {
  HttpClient
} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class WebhookService extends BaseApiService {

  _baseUrl = 'api/webhooks';

  constructor(
    httpClient: HttpClient
  ) {
    super(httpClient);
  }

  public clicked(emailId: string) {
    return this.doPost(`${this._baseUrl}/clicked`, {
      emailId: emailId
    });
  }
}
