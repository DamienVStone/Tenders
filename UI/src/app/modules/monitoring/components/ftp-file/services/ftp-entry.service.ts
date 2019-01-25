import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { IFilterOptions } from 'src/app/models/ifilter-options';
import { Observable, of } from 'rxjs';
import { IListResponse } from 'src/app/models/ilist-response';
import { IFtpEntry } from '../models/iftp-entry';
import { StateFile } from '../models/state-file.enum';
import { map } from 'rxjs/operators';

let date: Date = new Date();

let ftpFiles: IFtpEntry[] = [
  {
    id: "d6e9bc7e-0ae4-402d-973b-ceeef508622d1",
    createdDate: date,
    isActive: true,
    Size: 100,
    Name: "fsdgjngjfkdnhsjknhbj'fn",
    Modified: date,
    State: StateFile.Corrupted,
    Path: "C:/FTP/FTPParentPath/FTPChilsPath",
    Parent: " C:/FTP/FTPParentPath/",
    IsDirectory: false,
    IsArchive: false
  },
  {
    id: "d6e9bc7e-0ae4-402d-973b-ceeef508622d2",
    createdDate: date,
    isActive: true,
    Size: 200,
    Name: "fsdgjngjfkdnhsjknhbj'fn",
    Modified: date,
    State: StateFile.Modified,
    Path: "C:/FTP/FTPParentPath/FTPChilsPath",
    Parent: " C:/FTP/FTPParentPath/",
    IsDirectory: true,
    IsArchive: true
  },
  {
    id: "d6e9bc7e-0ae4-402d-973b-ceeef508622d3",
    createdDate: date,
    isActive: true,
    Size: 300,
    Name: "fsdgjngjfkdnhsjknhbj'fn",
    Modified: date,
    State: StateFile.Indexed,
    Path: "C:/FTP/FTPParentPath/FTPChilsPath",
    Parent: " C:/FTP/FTPParentPath/",
    IsDirectory: false,
    IsArchive: false
  },
  {
    id: "d6e9bc7e-0ae4-402d-973b-ceeef508622d4",
    createdDate: date,
    isActive: true,
    Size: 400,
    Name: "fsdgjngjfkdnhsjknhbj'fn",
    Modified: date,
    State: StateFile.Corrupted,
    Path: "C:/FTP/FTPParentPath/FTPChilsPath",
    Parent: " C:/FTP/FTPParentPath/",
    IsDirectory: true,
    IsArchive: true
  }
];

let listResponse: IListResponse<IFtpEntry[]> = {
  count: 10,
  data: ftpFiles
}


@Injectable({
  providedIn: 'root'
})
export class FtpEntryService {

  constructor(private http: HttpClient) { }
  url = `${environment.host}FTPFile/`;

  dataMock: Observable<IListResponse<IFtpEntry[]>>;

  // Метод для получения замоканых данных 
  get(filter: IFilterOptions): Observable<IListResponse<IFtpEntry[]>> {
    this.dataMock = of(listResponse);
    this.dataMock.pipe(map(this.parseDateTimeOffset));
    return this.dataMock;
  }

  // Метод посылает запрос на сервер (НЕ ИСПОЛЬЗОВАТЬ! пока нет идексов на файлах в БД)
  // get(filter: IFilterOptions): Observable<IListResponse<IFtpEntry[]>> {
  //   return this.http
  //     .get<IListResponse<IFtpEntry[]>>(this.url, { params: new HttpParams({ fromString: filter.toQueryString() }) })
  //     .pipe(map(this.parseDateTimeOffset));
  // }

  parseDateTimeOffset(resp: IListResponse<IFtpEntry[]>): IListResponse<IFtpEntry[]> {
    resp.data.forEach(element => {
      element.Modified = new Date(element.Modified)
    });
    return resp;
  }

}
