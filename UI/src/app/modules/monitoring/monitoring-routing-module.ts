import { Routes, RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { MonitoringComponent } from "./components/monitoring/monitoring.component";
import { MonitoringHomeComponent } from "./components/monitoring-home/monitoring-home.component";
import { FtpPathDetailComponent } from "./components/ftp-path/ftp-path-detail/ftp-path-detail.component";
import { FtpPathListComponent } from "./components/ftp-path/ftp-path-list/ftp-path-list.component";
import { FtpEntryListComponent } from "./components/ftp-file/ftp-entry-list/ftp-entry-list.component";

const monitoringRoutes: Routes = [
    {
        path: 'monitoring',
        component: MonitoringComponent,
        children: [
            {
                path: 'dashboard',
                component: MonitoringHomeComponent
            },
            {
                path: 'ftppath',
                component: FtpPathListComponent,
                children: [
                    {
                        path: ':id',
                        component: FtpPathDetailComponent
                    }
                ]
            },
            {
                path: 'ftpentry',
                component: FtpEntryListComponent
            }
        ]
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(monitoringRoutes)
    ],
    exports: [
        RouterModule
    ]
})
export class MonitoringRoutingModule {
}
