using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using MultichoiceCollection.Common.Entities.Base;
using MultichoiceCollection.Models.Repositories.Context;
using PagedList;

namespace MultichoiceCollection.Models.Repositories.Generic
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly AppDbContext _context;
        string _errorMessage = string.Empty;
        private IQueryable<TEntity> _query;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _query = _context.Set<TEntity>();
        }

        public TEntity Find<TKeyProperty>(TKeyProperty id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id), "id cannot be null.");
            return _context.Set<TEntity>().Find(id);
        }

        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate), "predicate cannot be null.");
            return _query.Where(predicate).FirstOrDefault();
        }

        public GenericRepository<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> include) where TProperty : new()
        {
            _query = _query.Include(include);
            return this;
        }

        public GenericRepository<TEntity> OrderBy<TProperty>(Expression<Func<TEntity, TProperty>> orderBy)
        {
            if (orderBy == null)
                throw new ArgumentNullException(nameof(orderBy), "orderBy cannot be null.");
            _query = _query.OrderBy(orderBy);
            return this;
        }

        public GenericRepository<TEntity> OrderByDescending<TProperty>(Expression<Func<TEntity, TProperty>> orderByDescending)
        {
            if (orderByDescending == null)
                throw new ArgumentNullException(nameof(orderByDescending), "orderByDescending cannot be null.");
            _query = _query.OrderByDescending(orderByDescending);
            return this;
        }

        public IPagedList<TEntity> GetAll(int page, int pageSize = 20)
        {
            return _query.ToPagedList(page, pageSize);
        }
        public IEnumerable<TEntity> GetAll()
        {
          //  var property = typeof(TEntity).GetProperties().FirstOrDefault(r => r.Name == "Id");
          //  Type propType = null;
          //if (property != null)
          //  {
          //      propType = property.PropertyType;
          //      if (_query.Count() > 5)
          //          return _query.OrderByDescending(e => propType.IsPrimitive).Take(50).ToList();
          //      }
            return _query.ToList();
           
        }
        public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate), "predicate cannot be null.");
            return _query.Where(predicate).ToList();
        }
        public IPagedList<TEntity> Where(Expression<Func<TEntity, bool>> predicate, int page, int pageSize = 20)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate), "predicate cannot be null.");
            return _query.Where(predicate).ToPagedList(page, pageSize);
        }
        
        public void Remove(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "entity cannot be null.");
            try
            {
                _context.Set<TEntity>().Remove(entity);
            }
            catch (DbEntityValidationException dbEx)
            {

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _errorMessage += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                throw new Exception(_errorMessage, dbEx);
            }

        }

        public void Remove<TKeyProperty>(TKeyProperty id)
        {
             try
            {
                var entity = Find(id);
                if (entity != null)
                    Remove(entity);
            }
            catch (DbEntityValidationException dbEx)
            {

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _errorMessage += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                throw new Exception(_errorMessage, dbEx);
            }

        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities), "entities cannot be null.");
            try
            {
                _context.Set<TEntity>().RemoveRange(entities);
            }
            catch (DbEntityValidationException dbEx)
            {

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _errorMessage += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                throw new Exception(_errorMessage, dbEx);
            }
        }

        public void Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "entity cannot be null.");
            try
            {
                _context.Set<TEntity>().Add(entity);
            }
            catch (DbEntityValidationException dbEx)
            {

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _errorMessage += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                throw new Exception(_errorMessage, dbEx);
            }
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities), "entities cannot be null.");
            try
            {
                _context.Set<TEntity>().AddRange(entities);
            }
            catch (DbEntityValidationException dbEx)
            {

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _errorMessage += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                throw new Exception(_errorMessage, dbEx);
            }
        }

        public int Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? _query.Count() : _query.Count(predicate);
        }

        public bool Contains(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "entity cannot be null.");

            try
            {
                return _query.Contains(entity);
            }
            catch (DbEntityValidationException dbEx)
            {

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _errorMessage += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                throw new Exception(_errorMessage, dbEx);
            }

        }
        public bool Contains(Expression<Func<TEntity, bool>> predicate)
        {
            return predicate != null && Find(predicate) !=null;
        }
        public bool Contains<TKeyProperty>(TKeyProperty id)
        {
           var entity = Find(id);
            return entity != null && Contains(entity);
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "entity cannot be null.");

            try
            {
                _context.Set<TEntity>().AddOrUpdate(entity);
            }
            catch (Exception)
            {
            }
           // _context.Entry(entity).State = EntityState.Modified;
           
        }

        public IPagedList<TEntity> Search(Expression<Func<TEntity, bool>> searchProperty, int page, int pageSize = 20)
        {
            if (searchProperty == null)
                throw new ArgumentNullException(nameof(searchProperty), "searchProperty cannot be null.");
            return _query.Where(searchProperty).ToPagedList(page,pageSize);
        }

        public IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>>searchProperty)
        {

            return _query.Where(searchProperty).ToList();
        }
    }
}