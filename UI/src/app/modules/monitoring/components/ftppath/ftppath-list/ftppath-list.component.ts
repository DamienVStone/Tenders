import { Component, OnInit } from '@angular/core';
import { FTPPathService } from '../services/ftppath.service';
import { IFTPPath } from '../models/iftp-path';
import { FilterOptions, IFilterOptions } from 'src/app/models/ifilter-options';
import { MatDialog, MatTableDataSource } from '@angular/material';
import { FTPPathDetailComponent } from '../ftppath-detail/ftppath-detail.component';
import { tap, switchMap } from 'rxjs/operators';
import { IListResponse } from 'src/app/models/ilist-response';
import { Observable, of } from 'rxjs';
import { NotificationService } from 'src/app/services/notification.service';

@Component({
  selector: 'app-ftppath-list',
  templateUrl: './ftppath-list.component.html',
  styleUrls: ['./ftppath-list.component.scss']
})
export class FTPPathListComponent implements OnInit {
  dataSource = new MatTableDataSource<IFTPPath>();
  isListLoading = false;
  filterOptions: IFilterOptions = new FilterOptions(0, 50);
  displayedColumns: string[] = ['id', 'path', 'login', 'password', 'lastTimeIndexed'];

  constructor(
    private ftppathService: FTPPathService,
    public notificationService: NotificationService,
    public dialog: MatDialog
  ) { }

  ngOnInit() {
    this.refreshList().subscribe();
  }

  applyFilter(filter: string) {
    this.filterOptions.globalFilter = filter.trim().toLowerCase();
  }

  refreshList(): Observable<IListResponse<IFTPPath[]>> {
    this.isListLoading = true;
    return this.ftppathService
      .get(this.filterOptions)
      .pipe(
        tap(
          result => {
            this.dataSource.data = result.data;
            this.isListLoading = false;
          },
          error => {
            this.notificationService.showError(error);
            this.isListLoading = false;
          }))
  }

  openDialog(ftpPath: IFTPPath): void {
    const dialogRef = this.dialog.open(
      FTPPathDetailComponent,
      {
        data: ftpPath
      });

    dialogRef.afterClosed().pipe(switchMap(c => c ? this.refreshList() : of(null))).subscribe();
  }
}