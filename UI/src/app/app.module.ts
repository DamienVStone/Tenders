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
import { FTPPathService } from './modules/monitoring/components/ftppath/services/ftppath.service';
import { MonitoringModule } from './modules/monitoring/monitoring.module';
import { NotificationService } from './services/notification.service';

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
  providers: [FTPPathService, NotificationService],
  bootstrap: [AppComponent]
})
export class AppModule { }

// platformBrowserDynamic().bootstrapModule(AppModule);