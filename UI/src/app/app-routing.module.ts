import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FTPPathDetailComponent } from './modules/monitoring/components/ftppath/ftppath-detail/ftppath-detail.component';

const routes: Routes = [
  // { path: 'monitoring', component: MonitoringComponent },
  { path: '', redirectTo: '/', pathMatch: 'full' }
  // { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }