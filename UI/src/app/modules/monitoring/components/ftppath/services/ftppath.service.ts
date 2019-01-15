import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { IFilterOptions } from 'src/app/models/ifilter-options';
import { IFTPPath } from '../models/IFTPPath';

@Injectable({
  providedIn: 'root'
})
export class FTPPathService {

  constructor(private http: HttpClient) { }

  get(filter: IFilterOptions): Observable<IFTPPath[]> {
    return this.http.get<IFTPPath[]>(environment.host + "FTPPath/", { params: new HttpParams({ fromString: filter.toQueryString() }) });
  }
}
