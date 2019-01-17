import { Component, OnInit, ViewChild } from '@angular/core';
import { IFtpPath } from '../models/iftp-path';
import { FilterOptions, IFilterOptions } from 'src/app/models/ifilter-options';
import { MatDialog, MatPaginator } from '@angular/material';
import { tap, switchMap, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { IListResponse } from 'src/app/models/ilist-response';
import { Observable, of } from 'rxjs';
import { NotificationService } from 'src/app/services/notification.service';
import { I18nService } from 'src/app/services/i18n.service';
import { FtpPathService } from '../services/ftp-path.service';
import { FtpPathDetailComponent } from '../ftp-path-detail/ftp-path-detail.component';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-ftppath-list',
  templateUrl: './ftp-path-list.component.html',
  styleUrls: ['./ftp-path-list.component.scss']
})
export class FtpPathListComponent implements OnInit {
  dataSource: IFtpPath[];
  dataLength = 0;
  isListLoading = false;
  filterOptions: IFilterOptions = new FilterOptions(0, 50);
  displayedColumns: string[] = ['id', 'path', 'login', 'password', 'lastTimeIndexed'];
  searchControl = new FormControl();

  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(
    private ftppathService: FtpPathService,
    private i18n: I18nService,
    public notificationService: NotificationService,
    public dialog: MatDialog
  ) { }

  ngOnInit() {
    this.i18n.matPaginator(this.paginator);
    this.paginator.page
      .pipe(tap(c => {
        this.filterOptions.page = c.pageIndex;
        this.filterOptions.pageSize = c.pageSize;
      }))
      .pipe(switchMap(c => this.refreshList()))
      .subscribe();

    this.searchControl.valueChanges
      .pipe(debounceTime(400))
      .pipe(distinctUntilChanged())
      .pipe(tap(filter => this.filterOptions.quickSearch = filter.trim().toUpperCase()))
      .pipe(switchMap(r => this.refreshList()))
      .subscribe();

    this.refreshList().subscribe();
  }

  refreshList(): Observable<IListResponse<IFtpPath[]>> {
    this.isListLoading = true;
    console.log(this.filterOptions);
    return this.ftppathService
      .get(this.filterOptions)
      .pipe(
        tap(
          result => {
            // this.dataSource.filteredData = result.data;
            this.dataSource = result.data;
            this.dataLength = result.count;
            this.isListLoading = false;
          },
          error => {
            this.notificationService.showError(error);
            this.isListLoading = false;
          }))
  }

  openDialog(ftpPath: IFtpPath): void {
    const dialogRef = this.dialog.open(
      FtpPathDetailComponent,
      {
        data: ftpPath
      });

    dialogRef.afterClosed().pipe(switchMap(c => c ? this.refreshList() : of(null))).subscribe();
  }
}