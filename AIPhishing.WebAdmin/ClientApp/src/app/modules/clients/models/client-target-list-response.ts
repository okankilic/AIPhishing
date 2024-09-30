import {
  ClientTargetListViewModel
} from "./client-target-list-view.model";

export interface ClientTargetListResponse {
  targets: ClientTargetListViewModel[];
  totalCount: number;
}
