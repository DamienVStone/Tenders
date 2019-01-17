import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { MatNativeDateModule } from '@angular/material';
import { MaterialModule } from './material-module';
import { SidenavComponent } from './modules/sidenav/sidenav.component';
import { MonitoringModule } from './modules/monitoring/monitoring.module';
import { NotificationService } from './services/notification.service';
import { FtpPathService } from './modules/monitoring/components/ftp-path/services/ftp-path.service';
import { FtpEntryService } from './modules/monitoring/components/ftp-file/services/ftp-entry.service';

@NgModule({
  declarations: [
    AppComponent,
    SidenavComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    HttpClientModule,
    MaterialModule,
    MatNativeDateModule,
    ReactiveFormsModule,
    MonitoringModule,
    AppRoutingModule
  ],
  providers: [FtpPathService, FtpEntryService, NotificationService],
  bootstrap: [AppComponent]
})
export class AppModule { }

// platformBrowserDynamic().bootstrapModule(AppModule);