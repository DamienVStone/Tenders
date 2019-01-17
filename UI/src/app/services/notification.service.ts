import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor(
    public snackBar: MatSnackBar
  ) { }

  public showError(error: any) {
    let message = 'Сообщение об ошибке не получено';
    if (error) {
      message = "Ошибка: ";
      if (error.error && typeof(error.error) == typeof('string')) {
        message += error.error;
      } else if (error.message) {
        message += error.message;
      } else {
        message += error;
      }
    }

    console.log(error);

    this.snackBar.open(message, null, { duration: 3000, panelClass: ['background-red'] });
  }
}
