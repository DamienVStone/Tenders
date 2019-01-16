import { Component, OnInit } from '@angular/core';
import { FTPPathService } from '../services/ftppath.service';
import { IFTPPath } from '../models/iftp-path';
import { FilterOptions, IFilterOptions } from 'src/app/models/ifilter-options';
import { MatDialog } from '@angular/material';
import { FTPPathDetailComponent } from '../ftppath-detail/ftppath-detail.component';
import { tap, switchMap } from 'rxjs/operators';
import { IListResponse } from 'src/app/models/ilist-response';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-ftppath-list',
  templateUrl: './ftppath-list.component.html',
  styleUrls: ['./ftppath-list.component.scss']
})
export class FTPPathListComponent implements OnInit {
  list: IFTPPath[] = [];
  isListLoading = false;
  filterOptions: IFilterOptions = new FilterOptions(1, 20);
  displayedColumns: string[] = ['id', 'path', 'login', 'password', 'lastTimeIndexed'];

  constructor(private ftppathService: FTPPathService, public dialog: MatDialog) { }

  ngOnInit() {
    this.refreshList().subscribe(c => { }, error => console.log(error));
  }

  refreshList(): Observable<IListResponse<IFTPPath[]>> {
    this.isListLoading = true;
    return this.ftppathService
      .get(this.filterOptions)
      .pipe(tap(result => {
        this.list = result.data;
        this.isListLoading = false;
      }))
  }

  openDialog(ftpPath: IFTPPath): void {
    const dialogRef = this.dialog.open(
      FTPPathDetailComponent,
      {
        // width: '250px',
        data: ftpPath
      });

    dialogRef.afterClosed().pipe(switchMap(c => this.refreshList())).subscribe(c => { }, error => console.log(error));
  }
}