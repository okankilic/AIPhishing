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
  MatTableDataSource
} from "@angular/material/table";
import {
  ClientListViewModel
} from "../models/client-list-view-model";
import {
  MatPaginator
} from "@angular/material/paginator";

@Component({
  selector: 'app-client-list',
  templateUrl: './client-list.component.html',
  styleUrls: ['./client-list.component.css']
})
export class ClientListComponent implements OnInit, AfterViewInit {

  dataSource = new MatTableDataSource<ClientListViewModel>([]);
  displayedColumns = [
    'clientName',
    'actions'
  ];

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private readonly _clientsService: ClientsService
  ) {
  }

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.paginator.page
      .subscribe(_ => {
        this.getList();
      });
    this.getList()
  }

  getList() {
    this._clientsService.list({
      pageSize: this.paginator.pageSize,
      currentPage: this.paginator.pageIndex + 1
    }).subscribe(response => {
      this.paginator.length = response.totalCount;
      this.dataSource.data = response.clients;
    })
  }
}
