import { Component, OnInit } from '@angular/core';
import { FTPPathService } from '../services/ftppath.service';
import { IFTPPath } from '../models/IFTPPath';
import { FilterOptions } from 'src/app/models/ifilter-options';

@Component({
  selector: 'app-ftppath-list',
  templateUrl: './ftppath-list.component.html',
  styleUrls: ['./ftppath-list.component.scss']
})
export class FTPPathListComponent implements OnInit {

  constructor(private ftppathService: FTPPathService) { }
  list: IFTPPath[] = [];
  page: number = 0;
  pageSize: number = 50;
  ngOnInit() {
    this.ftppathService.get(new FilterOptions(this.page, this.pageSize)).subscribe(result => this.list = result, error => console.log(error))
  }

}
