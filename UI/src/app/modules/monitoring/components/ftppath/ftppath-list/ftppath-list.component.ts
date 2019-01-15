import { Component, OnInit } from '@angular/core';
import { FTPPathService } from '../services/ftppath.service';
import { IFTPPath } from '../models/IFTPPath';
import { FilterOptions, IFilterOptions } from 'src/app/models/ifilter-options';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-ftppath-list',
  templateUrl: './ftppath-list.component.html',
  styleUrls: ['./ftppath-list.component.scss']
})
export class FTPPathListComponent implements OnInit {

  constructor(private ftppathService: FTPPathService) { }
  list: IFTPPath[] = [];
  filterOptions: IFilterOptions = new FilterOptions(1, 50);
  
  ngOnInit() {
    this.ftppathService
      .get(this.filterOptions)
      .subscribe(result => this.list = result, error => console.log(error));
  }

}
