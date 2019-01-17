import { Injectable } from '@angular/core';
import { MatPaginator } from '@angular/material';

@Injectable({
  providedIn: 'root'
})
export class I18nService {

  constructor() { }

  public matPaginator(paginator: MatPaginator) {
    paginator._intl.firstPageLabel = "Первая страница";
    paginator._intl.itemsPerPageLabel = "Размер страницы";
    paginator._intl.lastPageLabel = "Последняя страница";
    paginator._intl.nextPageLabel = "Следующая страница";
    paginator._intl.previousPageLabel = "Предыдущая страница";
    paginator._intl.getRangeLabel = (page, pageSize, length) => {
      if (length == 0 || pageSize == 0) {
        return "0 из " + length;
      }
      length = Math.max(length, 0);
      var startIndex = page * pageSize;
      var endIndex = startIndex < length ?
        Math.min(startIndex + pageSize, length) :
        startIndex + pageSize;
      return startIndex + 1 + " - " + endIndex + " из " + length;
    };
  }
}
