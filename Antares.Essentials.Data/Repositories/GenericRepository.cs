﻿using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Antares.Essentials.Data.Entities;

namespace Antares.Essentials.Data.Repositories
{
    public class GenericRepository<TEntity> : GenericRepository<TEntity, int>, IGenericRepository<TEntity>
        where TEntity : class, IEntity
    {
        public GenericRepository(DbContext context) : base(context) { }
    }

    public class GenericRepository<TEntity, TEntityKey> : IGenericRepository<TEntity, TEntityKey>
    where TEntity : class, IEntity<TEntityKey>
    {
        protected DbContext Db;
        protected DbSet<TEntity> DbSet;

        public GenericRepository(DbContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }

        public virtual TEntity GetById(TEntityKey id)
        {
            if (id == null) throw new ArgumentException("id");

            return DbSet.Find(id);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return DbSet.AsNoTracking();
        }

        public virtual void Add(TEntity obj)
        {
            if (obj == null) throw new ArgumentException("obj");

            DbSet.Add(obj);
        }

        public void AddOrUpdate(TEntity obj)
        {
            if (obj == null) throw new ArgumentException("obj");
            if(!DbSet.Any(e => e.Id.Equals(obj.Id)))
            {
                Add(obj);
            }
            else
            {
                Update(obj);
            }
        }

        public virtual void Update(TEntity obj)
        {
            if (obj == null) throw new ArgumentException("obj");

            DbSet.Update(obj);
        }

        public virtual void Remove(TEntityKey id)
        {
            DbSet.Remove(GetById(id));
        }

        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentException("predicate");

            return GetAll().Where(predicate);
        }

        public virtual int SaveChanges()
        {
            return Db.SaveChanges();
        }

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
