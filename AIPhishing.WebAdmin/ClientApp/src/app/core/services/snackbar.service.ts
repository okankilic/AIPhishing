import {
  Injectable,
  NgZone
} from '@angular/core';
import {
  MatSnackBar
} from '@angular/material/snack-bar';
import {
  equalsIgnoringCase
} from "../../shared/utils/string.utils";

@Injectable({
  providedIn: 'root'
})
export class SnackbarService {
  private snackBarSuccessStyle = 'mat-primary';
  private snackBarErrorStyle = 'mat-warn';
  private snackBarInformationStyle = 'mat-accent';

  constructor(private readonly snackBar: MatSnackBar,
              private readonly zone: NgZone) {

  }

  show(title: 'success' | 'Success' | 'information' | 'Information' | 'error' | 'Error', text: string) {

    const panelClass = equalsIgnoringCase(title, 'error')
      ? this.snackBarErrorStyle
      : equalsIgnoringCase(title, 'success')
        ? this.snackBarSuccessStyle
        : equalsIgnoringCase(title, 'information')
          ? this.snackBarInformationStyle
          : this.snackBarErrorStyle;

    this.zone.run(() => {

      const snackBar = this.snackBar.open(text, undefined, {
        panelClass: [
          panelClass
        ],
        verticalPosition: 'top',
        horizontalPosition: 'center',
        duration: 10000,
      });

      snackBar.onAction().subscribe(() => {
        snackBar.dismiss();
      });
    });
  }
}
