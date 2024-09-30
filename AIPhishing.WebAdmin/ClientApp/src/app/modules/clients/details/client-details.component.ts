import {
  AfterViewInit,
  Component,
  OnInit,
  ViewChild
} from '@angular/core';
import {
  ClientsService
} from "../services/clients.service";
import {
  ActivatedRoute
} from "@angular/router";
import {
  FormControl,
  FormGroup,
  Validators
} from "@angular/forms";
import {
  ClientViewModel
} from "../models/client-view.model";
import {
  MatTableDataSource
} from "@angular/material/table";
import {
  ClientTargetListViewModel
} from "../models/client-target-list-view.model";
import {
  MatPaginator
} from "@angular/material/paginator";
import {
  ClientUpdateRequest
} from "../models/client-update-request";
import {
  SnackbarService
} from "../../../core/services/snackbar.service";
import {
  ClientUserEditModel
} from "../models/client-user-edit.model";

@Component({
  selector: 'app-client-details',
  templateUrl: './client-details.component.html',
  styleUrls: ['./client-details.component.css']
})
export class ClientDetailsComponent implements OnInit, AfterViewInit {

  private _clientId: string = '';
  private _client: ClientViewModel | null = null;

  clientForm = new FormGroup({
    'clientName': new FormControl<string | null>('', [
      Validators.required,
      Validators.maxLength(255)
    ])
  });

  userForm = new FormGroup({
    'email': new FormControl<string | null>('', [
      Validators.required,
      Validators.email
    ]),
    'password': new FormControl<string | null>('', [
      Validators.required
    ])
  });

  csvForm = new FormGroup({
    'csvFile': new FormControl<File | null>(null, [
      Validators.required
    ]),
    'csvFileName': new FormControl<string>('', [
      Validators.required
    ])
  });

  dataSource = new MatTableDataSource<ClientTargetListViewModel>([]);
  displayedColumns = [
    'email',
    'fullName',
    'actions'
  ];

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private readonly _clientService: ClientsService,
    private _activatedRoute: ActivatedRoute,
    private readonly _snackbarService: SnackbarService
  ) {
    this._clientId = this._activatedRoute.snapshot.params.id;
  }

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.paginator.page
      .subscribe(_ => {
        this.getTargets();
      });
    this.getClient();
    this.getTargets();
  }

  onCsvFileChange(files: FileList | null) {
    if (files?.length! > 0) {
      const file = files!.item(0);
      this.csvForm.get('csvFile')?.setValue(file);
      this.csvForm.get('csvFileName')?.setValue(file!.name);
    }
  }

  onClientFormSubmit() {
    if (this.clientForm.invalid) {
      this.clientForm.markAllAsTouched();
      return;
    }
    const request : ClientUpdateRequest = Object.assign({}, this.clientForm.value);
    this._clientService.updateClient(this._clientId, request)
      .subscribe(() => {
        this._snackbarService.show('success', `Client updated.`);
        this.getClient();
      })
  }

  onUserFormSubmit() {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }
    const request : ClientUserEditModel = Object.assign({}, this.userForm.value);
    this._clientService.updateUser(this._clientId, request)
      .subscribe(() => {
        this._snackbarService.show('success', `Client user updated.`);
        this.getClient();
      })
  }

  onCsvFormSubmit() {
    if (this.csvForm.invalid) {
      this.csvForm.markAllAsTouched();
      return;
    }
    const formData = new FormData();
    formData.append('file', this.csvForm.value.csvFile ?? '');
    this._clientService.importTargets(this._clientId, formData)
      .subscribe(_ => {
        this.csvForm.patchValue({
          csvFile: null,
          csvFileName: null
        })
        this._snackbarService.show('success', 'Targets imported.');
        this.getTargets();
      })
  }

  onTargetDeleteClick(target: ClientTargetListViewModel) {
    this._clientService.deleteTarget(this._clientId, target.id)
      .subscribe( _=> {
        this._snackbarService.show('success', `Target ${target.email} deleted.`);
        this.getTargets();
      })
  }

  getClient() {
    this._clientService.get(this._clientId)
      .subscribe(response => {
        this._client = response;
        this.clientForm.patchValue({
          clientName: this._client.clientName
        })
        this.userForm.patchValue({
          email: this._client.user.email
        })
      });
  }

  getTargets() {
    this._clientService.listTargets(this._clientId, {
      pageSize: this.paginator.pageSize,
      currentPage: this.paginator.pageIndex + 1
    }).subscribe(response => {
      this.paginator.length = response.totalCount;
      this.dataSource.data = response.targets;
    })
  }
}
