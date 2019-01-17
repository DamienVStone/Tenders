using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Tenders.API.DAL.Interfaces;
using Tenders.API.Models;
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
            CheckId(Id);
            var update = Builders<T>.Update.Set(f => f.IsActive, IsActive);
            return Entities.FindOneAndUpdate(f => f.Id == Id, update) != null;
        }

        public long Count(int skip, int take, string quickSearch = "", bool isActive = true)
        {
            var filter = _filter(quickSearch, isActive);
            return filter.CountDocuments();
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

        public bool CreateMany(IEnumerable<T> Items)
        {
            Entities.InsertMany(Items);
            return true;
        }

        public bool Delete(string Id)
        {
            CheckId(Id);
            return Entities.DeleteOne(f => f.Id == Id).DeletedCount > 0;
        }

        public bool Exists(string Id, bool IsActive = true)
        {
            CheckId(Id);
            return Entities.CountDocuments(e => e.Id == Id && e.IsActive == IsActive) != 0;
        }

        public IEnumerable<T> Get(int skip, int take, string quickSearch, bool isActive = true)
        {
            var filter = _filter(quickSearch, isActive);
            var res = filter.Skip(skip)
                .Limit(take)
                .ToEnumerable();
            return res;
        }

        private IFindFluent<T, T> _filter(string quickSearch, bool IsActive)
        {
            quickSearch = quickSearch.ToSearchString() ?? string.Empty;
            return Entities
                .Find(f =>
                    f.IsActive == IsActive
                    && (quickSearch == null || quickSearch == "" || f.QuickSearch.Contains(quickSearch))
                );
        }


        public T GetOne(string Id)
        {
            CheckId(Id);
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

        protected void CheckId(string Id)
        {
            if (!IdProvider.IsIdValid(Id)) throw new ArgumentException("Некорректный идентификатор");
        }
    }
}
