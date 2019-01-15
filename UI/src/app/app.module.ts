import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { MatNativeDateModule } from '@angular/material';
import { MaterialModule } from './material-module';
import { SidenavComponent } from './modules/sidenav/sidenav.component';
import { MonitoringComponent } from './modules/monitoring/monitoring.component';
import { FTPPathListComponent } from './modules/monitoring/components/ftppath/ftppath-list/ftppath-list.component';
import { FTPPathService } from './modules/monitoring/components/ftppath/services/ftppath.service';

@NgModule({
  declarations: [
    AppComponent,
    SidenavComponent,
    MonitoringComponent,
    FTPPathListComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    MatNativeDateModule,
    ReactiveFormsModule,
    MaterialModule
  ],
  providers: [FTPPathService],
  bootstrap: [AppComponent]
})
export class AppModule { }

platformBrowserDynamic().bootstrapModule(AppModule);