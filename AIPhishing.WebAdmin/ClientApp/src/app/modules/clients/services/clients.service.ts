import {
  Injectable
} from '@angular/core';
import {
  BaseApiService
} from "../../../shared/services/base-api.service";
import {
  HttpClient
} from "@angular/common/http";
import {
  ClientListRequest
} from "../models/client-list-request";
import {
  ClientListResponse
} from "../models/client-list-response";
import {
  ClientViewModel
} from "../models/client-view.model";
import {
  ClientTargetListResponse
} from "../models/client-target-list-response";
import {
  ClientTargetListRequest
} from "../models/client-target-list-request";
import {
  ClientUpdateRequest
} from "../models/client-update-request";
import {
  ClientUserEditModel
} from "../models/client-user-edit.model";

@Injectable({
  providedIn: 'root'
})
export class ClientsService extends BaseApiService {

  _baseUrl = `api/clients`;

  constructor(
    httpClient: HttpClient
  ) {
    super(httpClient);
  }

  public create(request: FormData) {
    return this.doPost(`${this._baseUrl}`, request);
  }

  public list(request: ClientListRequest){
    return this.doGet<ClientListResponse>(`${this._baseUrl}`, {
      params: this.toHttpParams(request)
    })
  }

  public get(clientId: string) {
    return this.doGet<ClientViewModel>(`${this._baseUrl}/${clientId}`);
  }

  public listTargets(clientId: string, request: ClientTargetListRequest) {
    return this.doGet<ClientTargetListResponse>(`${this._baseUrl}/${clientId}/targets`, {
      params: this.toHttpParams(request)
    })
  }

  public updateClient(clientId: string, request: ClientUpdateRequest) {
    return this.doPut(`${this._baseUrl}/${clientId}`, request);
  }

  public updateUser(clientId: string, request: ClientUserEditModel) {
    return this.doPut(`${this._baseUrl}/${clientId}/user`, request);
  }

  public importTargets(clientId: string, request: FormData) {
    return this.doPut(`${this._baseUrl}/${clientId}/targets`, request);
  }

  public deleteTarget(clientId: string, targetId: string) {
    return this.doDelete(`${this._baseUrl}/${clientId}/targets/${targetId}`);
  }

  public downloadSampleTargetCsvFile() {
    return this.httpClient.get(`${this._appConfigService.appConfig.baseUrl}/${this._baseUrl}/sample-target-csv`, {
      responseType: 'blob'
    });
  }
}
