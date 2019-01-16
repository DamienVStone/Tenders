import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { IFilterOptions } from 'src/app/models/ifilter-options';
import { IFTPPath } from '../models/iftp-path';
import { IListResponse } from 'src/app/models/ilist-response';
import { IFTPPathParam } from '../models/iftp-path-params';

@Injectable({
  providedIn: 'root'
})
export class FTPPathService {
  constructor(private http: HttpClient) { }

  patch(data: IFTPPathParam): Observable<Object> {
    return this.http.patch(environment.host + "FTPPath/", data);
  }

  get(filter: IFilterOptions): Observable<IListResponse<IFTPPath[]>> {
    return this.http
      .get<IListResponse<IFTPPath[]>>(environment.host + "FTPPath/", { params: new HttpParams({ fromString: filter.toQueryString() }) })
      .pipe(map(this.parseDateTimeOffset));
  }

  parseDateTimeOffset(resp: IListResponse<IFTPPath[]>): IListResponse<IFTPPath[]> {
    resp.data.forEach(element => {
      element.lastTimeIndexed = new Date(element.lastTimeIndexed)
    });
    return resp;
  }
}
