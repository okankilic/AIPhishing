import {
  Component
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  Validators
} from "@angular/forms";
import {
  AuthService
} from "../services/auth.service";
import {
  AuthUpdatePasswordRequest
} from "../models/auth-update-password-request";
import {
  SnackbarService
} from "../../../core/services/snackbar.service";
import {
  AppContextService
} from "../../../core/services/app-context.service";

@Component({
  selector: 'app-auth-account',
  templateUrl: './auth-account.component.html',
  styleUrls: ['./auth-account.component.css']
})
export class AuthAccountComponent {

  form = new FormGroup({
    'oldPassword': new FormControl('', [
      Validators.required,
      Validators.maxLength(100)
    ]),
    'newPassword': new FormControl('', [
      Validators.required,
      Validators.maxLength(100)
    ])
  })

  constructor(
    private readonly _authService: AuthService,
    private readonly _snackbarService: SnackbarService,
    private _appContextService: AppContextService
  ) {
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const request: AuthUpdatePasswordRequest = Object.assign({}, this.form.value);
    this._authService.updatePassword(request)
      .subscribe(_ => {
        this._snackbarService.show('success', 'Password updated.');
        this._appContextService.logout();
      })
  }
}
