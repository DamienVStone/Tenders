import { IModelBase } from "src/app/models/imodel-base";

export interface IFtpPath extends IModelBase {
    path: string,
    login: string,
    password: string,
    hasErrors: boolean,
    lastTimeIndexed: Date
}
