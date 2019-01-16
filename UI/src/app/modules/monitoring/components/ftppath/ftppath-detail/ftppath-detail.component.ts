import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatSnackBar } from '@angular/material';
import { IFTPPath } from '../models/iftp-path';
import { FormGroup, FormControl } from '@angular/forms';
import * as moment from 'moment/moment';
import { FTPPathService } from '../services/ftppath.service';
import { NotificationService } from 'src/app/services/notification.service';

@Component({
  selector: 'app-ftppath-detail',
  templateUrl: './ftppath-detail.component.html',
  styleUrls: ['./ftppath-detail.component.scss']
})
export class FTPPathDetailComponent {
  isLoading = false;
  ftpPathForm: FormGroup;
  constructor(
    private ftpPathService: FTPPathService,
    public notificationService: NotificationService,
    public dialogRef: MatDialogRef<FTPPathDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public data: IFTPPath
  ) {
    this.dialogRef.disableClose = true;
    this.ftpPathForm = new FormGroup({
      id: new FormControl({ value: data.id, disabled: true }),
      path: new FormControl(data.path),
      login: new FormControl(data.login),
      password: new FormControl(data.password),
      lastTimeIndexed: new FormControl({ value: moment(data.lastTimeIndexed).format('YYYY-MM-DD[T]HH:mm:ss'), disabled: true })
    });
  }

  onClose() {
    this.dialogRef.close(false);
  }

  onSubmit() {
    let v = this.ftpPathForm.value;
    v.id = this.data.id;
    this.isLoading = true;
    this.ftpPathService
      .patch(v)
      .subscribe(
        success => {
          console.log(success);
          this.dialogRef.close(true);
          this.isLoading = false;
        },
        error => {
          console.log(error);
          this.notificationService.showError(error);
          this.isLoading = false;
        }
      );
  }
}
