using MongoDB.Driver;
using System;
using System.Collections.Generic;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;

namespace Tenders.API.DAL.Mongo
{
    public abstract class BaseMongoRepo<T> : IAPIRepository<T> where T : ModelBase
    {
        protected IIdProvider IdProvider;

        public BaseMongoRepo(IIdProvider idProvider)
        {
            IdProvider = idProvider;
        }

        protected abstract IMongoCollection<T> Entities { get; }

        protected abstract UpdateDefinition<T> createUpdateDefinition(T db, T inp);

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
            return Entities.Find(f => f.IsActive == IsActive).Skip(Skip).Limit(Take).ToEnumerable();
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
            var update = createUpdateDefinition(GetOne(Item.Id), Item);
            return Entities.FindOneAndUpdate(f => f.IsActive && f.Id == Item.Id, update)!=null;
        }

        protected void checkId(string Id)
        {
            if (!IdProvider.IsIdValid(Id)) throw new ArgumentException("Некорректный идентификатор");
        }
        
    }
}
