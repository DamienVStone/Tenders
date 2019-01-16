import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { IFTPPath } from '../models/iftp-path';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-ftppath-detail',
  templateUrl: './ftppath-detail.component.html',
  styleUrls: ['./ftppath-detail.component.scss']
})
export class FTPPathDetailComponent {
  ftpPathForm = new FormGroup({
    id: new FormControl(''),
    path: new FormControl(''),
    login: new FormControl(''),
    password: new FormControl(''),
    lastTimeIndexed: new FormControl(''),
    hasErrors: new FormControl(''),
  });

  constructor(
    public dialogRef: MatDialogRef<FTPPathDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public data: IFTPPath) {
    this.ftpPathForm.controls['id'].setValue(data.id);
    this.ftpPathForm.controls['path'].setValue(data.path);
    this.ftpPathForm.controls['login'].setValue(data.login);
    this.ftpPathForm.controls['password'].setValue(data.password);
    this.ftpPathForm.controls['lastTimeIndexed'].setValue(data.lastTimeIndexed);
    this.ftpPathForm.controls['hasErrors'].setValue(data.hasErrors);
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  onSubmit() {
    console.log(this.ftpPathForm.value);
    debugger;
  }
}
