import {
  HttpClient,
  HttpParams
} from "@angular/common/http";
import {
  ApiResponse
} from "../api-response";
import {
  map
} from "rxjs";
import {
  AppConfigurationService
} from "./app-configuration.service";
import {
  inject
} from "@angular/core";

export abstract class BaseApiService {

  abstract readonly _baseUrl: string;

  protected readonly _appConfigService = inject(AppConfigurationService);

  constructor(
    protected readonly httpClient: HttpClient) {
  }

  protected doPost<TRes>(url: string, model: {}, options?: {}) {
    return this.httpClient.post<ApiResponse<TRes>>(this.changeUrl(url), model, options)
      .pipe(
        map((result: ApiResponse<TRes>) => result.result)
      );
  }

  protected doGet<TRes>(url: string, options?: {}) {
    return this.httpClient.get<ApiResponse<TRes>>(this.changeUrl(url), options)
      .pipe<TRes>(
        map((result: ApiResponse<TRes>) => result.result)
      );
  }

  protected doPut<TRes>(url: string, model: {}, options?: {}) {
    return this.httpClient.put<ApiResponse<TRes>>(this.changeUrl(url), model, options)
      .pipe<TRes>(
        map((result: ApiResponse<TRes>) => result.result)
      );
  }

  protected doDelete<TRes>(url: string) {
    return this.httpClient.delete<ApiResponse<TRes>>(this.changeUrl(url))
      .pipe<TRes>(
        map((result: ApiResponse<TRes>) => result.result)
      );
  }

  protected toFormData(obj: any): FormData {
    const formData = new FormData();
    Object.keys(obj)
      .forEach(key => {
        if (obj[key]) {
          formData.append(key, obj[key]);
        }
      });
    return formData;
  }

  protected toHttpParams(obj: any): HttpParams {
    let params = new HttpParams();
    Object.keys(obj).forEach(key => {
      if (obj[key]) {
        if (Array.isArray(obj[key])) {
          for (let val of obj[key]) {
            params = params.append(key, val);
          }
        } else {
          params = params.append(key, obj[key]);
        }
      }
    });
    return params;
  }

  protected changeUrl(url: string) {
    return this._appConfigService.appConfig.baseUrl + '/' + url;
  }
}
