import { Routes, RouterModule } from "@angular/router";
import { FTPPathListComponent } from "./components/ftppath/ftppath-list/ftppath-list.component";
import { NgModule } from "@angular/core";
import { MonitoringComponent } from "./components/monitoring/monitoring.component";
import { MonitoringHomeComponent } from "./components/monitoring-home/monitoring-home.component";
import { FTPPathDetailComponent } from "./components/ftppath/ftppath-detail/ftppath-detail.component";

const monitoringRoutes: Routes = [
    {
        path: 'monitoring',
        component: MonitoringComponent,
        children: [
            {
                path: 'ftppath',
                component: FTPPathListComponent,
                children: [
                    {
                        path: ':id',
                        component: FTPPathDetailComponent
                    }
                ]
            },
            {
                path: '',
                component: MonitoringHomeComponent
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
