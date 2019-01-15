import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FTPPathDetailComponent } from './components/ftppath/ftppath-detail/ftppath-detail.component';
import { FormsModule } from '@angular/forms';
import { MonitoringRoutingModule } from './monitoring-routing-module';
import { FTPPathListComponent } from './components/ftppath/ftppath-list/ftppath-list.component';
import { MonitoringComponent } from './components/monitoring/monitoring.component';
import { MonitoringHomeComponent } from './components/monitoring-home/monitoring-home.component';
import { MaterialModule } from 'src/app/material-module';

@NgModule({
  declarations: [
    MonitoringComponent,
    MonitoringHomeComponent,
    FTPPathListComponent,
    FTPPathDetailComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    MonitoringRoutingModule,
    MaterialModule
  ]
})
export class MonitoringModule { }
