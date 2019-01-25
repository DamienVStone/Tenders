import { StateFile } from "./state-file.enum";
import { IModelBase } from "src/app/models/imodel-base";

export interface IFtpEntry extends IModelBase {
    /**Размер */
    Size: number;
    /**Название */
    Name: string;
    /**Дата изменения */
    Modified: Date;
    /**Свойства */
    State: StateFile;
    /**Ссылка на путь */
    Path: string;
    /**Ссылка на родителя  */
    Parent: string;
    /**Является ли этот элемент дерикторией */
    IsDirectory: boolean;
    /**Является ли элемент архивом */
    IsArchive: boolean;
}