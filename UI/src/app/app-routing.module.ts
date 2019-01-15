import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MonitoringComponent } from './modules/monitoring/monitoring.component';

const routes: Routes = [
  { path: 'monitoring', component: MonitoringComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
