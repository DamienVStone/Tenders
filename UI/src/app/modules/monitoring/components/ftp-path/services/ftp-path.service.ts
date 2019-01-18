import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { IFilterOptions } from 'src/app/models/ifilter-options';
import { IFtpPath } from '../models/iftp-path';
import { IListResponse } from 'src/app/models/ilist-response';
import { IFtpPathParam } from '../models/iftp-path-params';

@Injectable({
  providedIn: 'root'
})
export class FtpPathService {

  constructor(private http: HttpClient) { }
  url = `${environment.host}FTPPath/`;

  create(data: IFtpPathParam): Observable<Object> {
    return this.http.post(this.url, data);
  }

  delete(id: string): Observable<Object> {
    return this.http.delete(`${this.url}?id=${id}`);
  }

  patch(data: IFtpPathParam): Observable<Object> {
    return this.http.patch(this.url, data);
  }

  get(filter: IFilterOptions): Observable<IListResponse<IFtpPath[]>> {
    return this.http
      .get<IListResponse<IFtpPath[]>>(this.url, { params: new HttpParams({ fromString: filter.toQueryString() }) })
      .pipe(map(this.parseDateTimeOffset));
  }

  parseDateTimeOffset(resp: IListResponse<IFtpPath[]>): IListResponse<IFtpPath[]> {
    resp.data.forEach(element => {
      element.lastTimeIndexed = new Date(element.lastTimeIndexed)
    });
    return resp;
  }
}
