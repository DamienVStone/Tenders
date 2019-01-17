import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { IFtpPath } from '../models/iftp-path';
import { FormGroup, FormControl } from '@angular/forms';
import * as moment from 'moment/moment';
import { FtpPathService } from '../services/ftp-path.service';
import { NotificationService } from 'src/app/services/notification.service';

@Component({
  selector: 'app-ftppath-detail',
  templateUrl: './ftp-path-detail.component.html',
  styleUrls: ['./ftp-path-detail.component.scss']
})
export class FtpPathDetailComponent {
  isLoading = false;
  ftpPathForm: FormGroup;
  constructor(
    private ftpPathService: FtpPathService,
    public notificationService: NotificationService,
    public dialogRef: MatDialogRef<FtpPathDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public data: IFtpPath
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
        },
        error => {
          console.log(error);
          this.notificationService.showError(error);
          this.isLoading = false;
        }
      );
  }


}
