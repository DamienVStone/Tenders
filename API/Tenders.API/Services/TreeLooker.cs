using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using TenderPlanAPI.Controllers;
using TenderPlanAPI.Enums;
using TenderPlanAPI.Models;
using TenderPlanAPI.Parameters;
using Tenders.API.Services.Interfaces;

namespace TenderPlanAPI.Services
{
    public class TreeLookerService: ITreeLookerService
    {
        private ObjectId _pathId; //идентификатор FTP пути по которому лежит дерево
        private List<FTPEntry> _dbFiles; //список файлов, полученных из базы данных
        private List<FTPEntryParam> _inputFiles; //список файлов, полученных из тела запроса
        private readonly IDBConnectContext dbContext;

        /// <summary>
        /// Класс, рекурсивно обходящий и индексирующий деревья файлов
        /// </summary>
        /// <param name="PathId">идентификатор FTP пути по которому лежит дерево</param>
        /// <param name="DbFiles">список файлов, полученных из базы данных</param>
        /// <param name="InputFiles">список файлов, полученных из тела запроса</param>
        public TreeLookerService(ObjectId PathId, List<FTPEntry> DbFiles, List<FTPEntryParam> InputFiles, IDBConnectContext DbContext)
        {
            _pathId = PathId;
            _dbFiles = DbFiles;
            _inputFiles = InputFiles;
            dbContext = DbContext ?? throw new ArgumentNullException(nameof(DbContext));
            _dbFiles.AsParallel().ForAll(f => f.State = StateFile.Pending);
        }

        /// <summary>
        /// Функция, обновляющая все файлы внутри очередной папки. Вызывается рекурсивно.
        /// </summary>
        /// <param name="dbParentId">Идентификатор папки базы данных внутри которой проиндексируются файлы</param>
        /// <param name="inputParentId">Идентификатор папки на входящем дереве с файлами из которой будет происходить сравнение</param>
        public void UpdateFiles(string dbParentId, string inputParentId)
        {
            var key = new object();
            var dbFolderContents = _dbFiles.Where(f => f.Parent.Equals(dbParentId)).ToDictionary(f => f.Name);
            var inputFolderContents = _inputFiles.Where(f => f.Parent.Equals(inputParentId)).ToList();

            inputFolderContents
                .AsParallel()
                .ForAll(f =>
                {
                    if (dbFolderContents.Keys.Contains(f.Name))
                    {
                        //Файл существует в базе
                        var dbFile = dbFolderContents[f.Name];

                        var updates = new List<UpdateDefinition<FTPEntry>>();
                        var isFileModified = false;

                        if (dbFile.Size != f.Size)
                        {
                            updates.Add(Builders<FTPEntry>.Update.Set("Size", f.Size));
                            isFileModified = true;
                        }
                        if (!dbFile.Modified.Equals(f.DateModified))
                        {
                            updates.Add(Builders<FTPEntry>.Update.Set("Modified", f.DateModified));
                            isFileModified = true;
                        }

                        var filter = Builders<FTPEntry>.Filter.Eq("_id", dbFile.Id);
                        if (isFileModified)
                        {
                            lock (key)
                            {
                                dbFolderContents[f.Name].State = StateFile.Modified;
                            }
                            updates.Add(Builders<FTPEntry>.Update.Set("State", StateFile.Modified));
                            dbContext.FTPEntry.UpdateOne(filter, Builders<FTPEntry>.Update.Combine(updates));
                        }
                        else
                        {
                            lock (key)
                            {
                                dbFolderContents[f.Name].State = StateFile.Indexed;
                            }
                        }

                        // Вызываю ту же функцию для данного файла, чтобы проиндексироваьт всех детей
                        UpdateFiles(dbFile.Id, f.Id);
                    }
                    else
                    {
                        //Файл не существует в базе
                        var newFile = new FTPEntry
                        {
                            Name = f.Name,
                            Modified = f.DateModified,
                            Size = f.Size,
                            Path = _pathId,
                            Parent = dbParentId
                        };

                        dbContext.FTPEntry.InsertOne(newFile);

                        // Вызываю ту же функцию для данного файла, чтобы проиндексироваьт всех детей
                        UpdateFiles(newFile.Id, f.Id);
                    }

                });

            var forDelete = dbFolderContents.Values.Where(f => f.State == StateFile.Pending).Select(f => Builders<FTPEntry>.Filter.Eq("_id", f.Id)).ToArray();
            if (forDelete.Length > 0) dbContext.FTPEntry.DeleteMany(Builders<FTPEntry>.Filter.Or(forDelete));
        }
    }
}
