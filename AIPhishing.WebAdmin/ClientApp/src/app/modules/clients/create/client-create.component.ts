import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  Validators
} from "@angular/forms";
import {
  ClientsService
} from "../services/clients.service";
import {
  AppContextService
} from "../../../core/services/app-context.service";
import {
  SnackbarService
} from "../../../core/services/snackbar.service";

@Component({
  selector: 'app-client-create',
  templateUrl: './client-create.component.html',
  styleUrls: ['./client-create.component.css']
})
export class ClientCreateComponent {

  userForm = new FormGroup({
    'email': new FormControl('', [
      Validators.required,
      Validators.email,
      Validators.maxLength(255)
    ]),
    'password': new FormControl('', [
      Validators.required,
      Validators.maxLength(100)
    ])
  });

  form = new FormGroup({
    'clientName': new FormControl('', [
      Validators.required,
      Validators.maxLength(255)
    ]),
    'user': this.userForm,
    'csvFile': new FormControl<File | null>(null, [
      Validators.required
    ]),
    'csvFileName': new FormControl<string>('', [
      Validators.required
    ]),
  })

  constructor(
    private readonly _clientsService: ClientsService,
    private _appContextService: AppContextService,
    private readonly _snackbarService: SnackbarService
  ) {
  }

  onCsvFileChange(files: FileList | null) {
    if (files?.length! > 0) {
      const file = files!.item(0);
      this.form.get('csvFile')?.setValue(file);
      this.form.get('csvFileName')?.setValue(file!.name);
    }
  }

  onSubmit() {
    if (this.form.invalid){
      this.form.markAllAsTouched();
      return;
    }
    const formData = new FormData();
    formData.append('clientName', this.form?.value?.clientName ?? '');
    formData.append('csvFile', this.form?.value?.csvFile ?? '');
    formData.append('user.email', this.form?.value?.user?.email ?? '');
    formData.append('user.password', this.form?.value?.user?.password ?? '');
    this._clientsService.create(formData)
      .subscribe(_ => {
        this._snackbarService.show('success', `Client created.`);
        this._appContextService.navigate('/');
      })
  }
}
