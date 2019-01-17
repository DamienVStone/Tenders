import { KeyValuePair } from "./key-value-pair";

export interface IFilterOptions {
    page: number,
    pageSize: number,
    quickSearch: string,
    filter: KeyValuePair<string, string>[],
    toQueryString(): string
}

export class FilterOptions implements IFilterOptions {
    constructor(page: number, pageSize: number) {
        this.page = page;
        this.pageSize = pageSize;
    }
    page: number;
    pageSize: number;
    quickSearch: string;
    filter: KeyValuePair<string, string>[];
    toQueryString(): string {
        return "page=" + this.page + "&pageSize=" + this.pageSize;
    }
}

