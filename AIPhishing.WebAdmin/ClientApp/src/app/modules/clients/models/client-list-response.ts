import {
  ClientListViewModel
} from "./client-list-view-model";

export interface ClientListResponse {
  clients: ClientListViewModel[];
  totalCount: number;
}
