import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MonitoringRoutingModule } from './monitoring-routing-module';
import { MonitoringComponent } from './components/monitoring/monitoring.component';
import { MonitoringHomeComponent } from './components/monitoring-home/monitoring-home.component';
import { MaterialModule } from 'src/app/material-module';
import { FtpPathListComponent } from './components/ftp-path/ftp-path-list/ftp-path-list.component';
import { FtpPathDetailComponent } from './components/ftp-path/ftp-path-detail/ftp-path-detail.component';
import { IFtpFileComponent } from './components/ftp-file/models/iftp-file/iftp-file.component';
import { FtpEntryListComponent } from './components/ftp-file/ftp-entry-list/ftp-entry-list.component';

@NgModule({
  declarations: [
    MonitoringComponent,
    MonitoringHomeComponent,
    FtpPathListComponent,
    FtpPathDetailComponent,
    FtpEntryListComponent,
    IFtpFileComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MonitoringRoutingModule,
    MaterialModule
  ]
})
export class MonitoringModule { }
