﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;
using Tenders.Core.Abstractions.Services;

namespace Tenders.API.DAL.Mongo
{
    public abstract class BaseMongoRepo<T> : IAPIRepository<T> where T : ModelBase
    {
        protected readonly IIdProvider IdProvider;
        protected readonly ILoggerService Logger;

        public BaseMongoRepo(IIdProvider idProvider, ILoggerService Logger)
        {
            IdProvider = idProvider ?? throw new ArgumentNullException(nameof(idProvider));
            this.Logger = Logger ?? throw new ArgumentNullException(nameof(Logger));
        }

        protected abstract IMongoCollection<T> Entities { get; }

        public bool ChangeActiveFlag(string Id, bool IsActive)
        {
            checkId(Id);
            var update = Builders<T>.Update.Set(f => f.IsActive, IsActive);
            return Entities.FindOneAndUpdate(f => f.Id == Id, update) != null;
        }

        public long CountAll(bool IsActive = true)
        {
            return Entities.CountDocuments(filter => filter.IsActive == IsActive);
        }

        public string Create(T Item)
        {
            if (!IdProvider.IsIdValid(Item.Id)) Item.Id = IdProvider.GenerateId();
            Entities.InsertOne(Item);
            return Item.Id;
        }

        public bool Delete(string Id)
        {
            checkId(Id);
            return Entities.DeleteOne(f => f.Id == Id).DeletedCount>0;
        }

        public bool Exists(string Id, bool IsActive = true)
        {
            checkId(Id);
            return Entities.CountDocuments(e => e.Id == Id && e.IsActive == IsActive) != 0;
        }

        public IEnumerable<T> Get(int Skip, int Take, bool IsActive = true)
        {
            Logger.Log($"Возврщаю список объектов типа {typeof(T)} c {Skip} по {Take} где IsActive = {IsActive}");
            var res = Entities.Find(f => f.IsActive == IsActive).Skip(Skip).Limit(Take).ToEnumerable();
            Logger.Log($"Успешно выбрал {res.Count()} объектов");
            return res;
        }

        public T GetOne(string Id)
        {
            checkId(Id);
            return Entities.Find(f => f.Id == Id).FirstOrDefault();
        }

        public bool Update(T Item)
        {
            if (Item == null) throw new ArgumentNullException(nameof(Item));
            if (!Exists(Item.Id)) throw new ArgumentException("Объект еще не создан");
            var res = Entities.ReplaceOne(f => f.IsActive && f.Id == Item.Id, Item);
            return res.IsModifiedCountAvailable && res.ModifiedCount>0;
        }

        protected void checkId(string Id)
        {
            if (!IdProvider.IsIdValid(Id)) throw new ArgumentException("Некорректный идентификатор");
        }
        
    }
}