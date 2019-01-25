export enum StateFile {
    /**В Ожидании обработки */
    Pending = -1,
    /**Новый */
    New = 0,
    /**Изменен */
    Modified = 1,
    /**Проиндексирован */
    Indexed = 2,
    /**Поврежден */
    Corrupted = 3
}