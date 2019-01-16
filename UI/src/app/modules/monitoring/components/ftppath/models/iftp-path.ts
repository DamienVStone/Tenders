import { IModelBase } from "src/app/models/imodel-base";

export interface IFTPPath extends IModelBase {
    path: string,
    login: string,
    password: string,
    hasErrors: boolean,
    lastTimeIndexed: Date
}
