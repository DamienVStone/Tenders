using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.Helpers;

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
            Item.GenerateQuickSearchString();
            Entities.InsertOne(Item);
            return Item.Id;
        }

        public bool Delete(string Id)
        {
            checkId(Id);
            return Entities.DeleteOne(f => f.Id == Id).DeletedCount > 0;
        }

        public bool Exists(string Id, bool IsActive = true)
        {
            checkId(Id);
            return Entities.CountDocuments(e => e.Id == Id && e.IsActive == IsActive) != 0;
        }

        public IEnumerable<T> Get(int skip, int take, string quickSearch, bool IsActive = true)
        {
            quickSearch = quickSearch.ToSearchString();
            Logger.Log($"Возврщаю список объектов типа {typeof(T)} c {skip} по {take} где IsActive = {IsActive}");
            var res = Entities
                .Find(f => f.IsActive == IsActive &&
                    (
                        quickSearch == null || quickSearch == "" || f.QuickSearch == null || f.QuickSearch == "" || f.QuickSearch.Contains(quickSearch))
                    )
                .Skip(skip)
                .Limit(take)
                .ToEnumerable();
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
            Item.GenerateQuickSearchString();
            var res = Entities.ReplaceOne(f => f.IsActive && f.Id == Item.Id, Item);
            return res.IsModifiedCountAvailable && res.ModifiedCount > 0;
        }

        protected void checkId(string Id)
        {
            if (!IdProvider.IsIdValid(Id)) throw new ArgumentException("Некорректный идентификатор");
        }
    }
}
