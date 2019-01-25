import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { IListResponse } from 'src/app/models/ilist-response';
import { IFtpEntry } from '../models/iftp-entry';
import { FtpEntryService } from '../services/ftp-entry.service';
import { IFilterOptions, FilterOptions } from 'src/app/models/ifilter-options';
import { tap } from 'rxjs/operators';
import { NotificationService } from 'src/app/services/notification.service';

@Component({
  selector: 'app-ftp-entry-list',
  templateUrl: './ftp-entry-list.component.html',
  styleUrls: ['./ftp-entry-list.component.scss']
})
export class FtpEntryListComponent implements OnInit {
  isListLoading = false;
  filterOptions: IFilterOptions = new FilterOptions(0, 50);
  dataSource: IFtpEntry[];
  dataLength = 0;
  displayedColumns: string[] = ['id', 'name', 'size', 'modified', 'state', 'path', 'parent', 'isDirectory', 'isArchive'];


  constructor(private ftpentryService: FtpEntryService, public notificationService: NotificationService) { }

  ngOnInit() {
    this.refreshList().subscribe();
    console.log(this.dataSource);
  }

  refreshList(): Observable<IListResponse<IFtpEntry[]>> {
    this.isListLoading = true;

    var list = this.ftpentryService.get(this.filterOptions).pipe(tap(result => {
      this.dataSource = result.data;
      this.dataLength = result.count;
      this.isListLoading = false;
    },
      error => {
        this.notificationService.showError(error);
        this.isListLoading = false;
      }));
    return list;
  }
}
