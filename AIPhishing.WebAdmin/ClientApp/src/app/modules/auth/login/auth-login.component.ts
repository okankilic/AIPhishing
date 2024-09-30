import {
  Component,
  OnInit
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  Validators
} from "@angular/forms";
import {
  AuthLoginRequest
} from "../models/auth-login-request";
import {
  AuthService
} from "../services/auth.service";
import {
  AppContextService
} from "../../../core/services/app-context.service";
import {
  ActivatedRoute
} from "@angular/router";
import {
  SnackbarService
} from "../../../core/services/snackbar.service";

@Component({
  selector: 'app-auth-login',
  templateUrl: './auth-login.component.html',
  styleUrls: ['./auth-login.component.css']
})
export class AuthLoginComponent implements OnInit {

  form = new FormGroup<any>({
    userName: new FormControl('', [
      Validators.required,
      Validators.maxLength(255),
      Validators.email
    ]),
    password: new FormControl('', [
      Validators.required,
      Validators.maxLength(100)
    ])
  });

  constructor(
    private readonly _authService : AuthService,
    private readonly _appContextService: AppContextService,
    private readonly _snackbarService: SnackbarService,
    private route: ActivatedRoute
  ) {
  }

  ngOnInit() {
    // this._appContextService.logout();
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const request : AuthLoginRequest = Object.assign({}, this.form.value);
    this._authService.login(request)
      .subscribe(response => {
        const returnUrl = this.route.snapshot.queryParams['returnUrl'];
        if (response && response.apiKey) {
          this._appContextService.setLoginResponse(response, returnUrl)
        }
      }, (err) => {
        if(err?.error?.errorMessage) {
          this._snackbarService.show('error', err.error.errorMessage);
        }
      });
  }
}
