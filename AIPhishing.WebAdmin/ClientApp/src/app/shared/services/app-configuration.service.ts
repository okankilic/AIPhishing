import {
  Injectable
} from '@angular/core';
import {
  HttpClient
} from "@angular/common/http";
import {
  AppConfiguration
} from "../models/app-configuration";

@Injectable({
  providedIn: 'root'
})
export class AppConfigurationService {

  private _appConfig!: AppConfiguration;

  get appConfig() {
    return this._appConfig;
  }

  constructor(
    private readonly _httpClient: HttpClient
  ) { }

  load() {
    return new Promise<void>((resolve, reject) => {
      this._httpClient.get<AppConfiguration>('/assets/config.json')
        .subscribe(config => {
          this._appConfig = config;
          resolve();
        })
    })
  }
}
