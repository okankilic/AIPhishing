import {
  ClientUserViewModel
} from "./client-user-view.model";

export interface ClientViewModel {
  id: string;
  clientName: string;
  user: ClientUserViewModel;
}
