import {
  Component
} from '@angular/core';
import {
  AppContextService
} from "./core/services/app-context.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';
  constructor(
    public readonly appContextService: AppContextService
  ) {
  }
}
