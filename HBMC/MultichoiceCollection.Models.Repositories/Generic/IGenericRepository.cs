using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using MultichoiceCollection.Common.Entities.Base;
using PagedList;

namespace MultichoiceCollection.Models.Repositories.Generic
{
    /// <summary>
    /// OfflineDigitalPay.Models.Repositories.Generic.IGenericRepository
    /// </summary>
    /// <typeparam name="TEntity">The enetity to be used in the repository</typeparam>
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Returns an entity with the supplied primary key
        /// </summary>
        /// <param name="id">The primary key</param>
        /// <returns>BaseEntity</returns>
        [CanBeNull]
        [System.Diagnostics.Contracts.Pure]
        TEntity Find<TKeyProperty>(TKeyProperty id);

        /// <summary>
        /// Returns an entity that matches the supplied predicate(condition)
        /// Throws ArgumentNullException if predicate is null
        /// </summary>
        /// <param name="predicate">the condition</param>
        /// <returns>BaseEntity</returns>
        [CanBeNull]
        [System.Diagnostics.Contracts.Pure]
        TEntity Find([NotNull]Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Includes the entities in the expression provided
        /// </summary>
        /// <param name="include">the condition</param>
        [System.Diagnostics.Contracts.Pure]
        GenericRepository<TEntity> Include<TProperty>([NotNull]Expression<Func<TEntity, TProperty>> include) where TProperty:new();

        /// <summary>
        /// Orders the entities in the expression provided in ascending order
        /// </summary>
        /// <param name="orderBy">the condition</param>
        [System.Diagnostics.Contracts.Pure]
        GenericRepository<TEntity> OrderBy<TProperty>([NotNull]Expression<Func<TEntity, TProperty>> orderBy);

        /// <summary>
        /// Orders the entities in the expression provided in descending order
        /// </summary>
        /// <param name="orderByDescending">the condition</param>
        [System.Diagnostics.Contracts.Pure]
        GenericRepository<TEntity> OrderByDescending<TProperty>([NotNull]Expression<Func<TEntity, TProperty>> orderByDescending);

        /// <summary>
        /// Returns a collection of all BaseEntity of TEntity in the DbSet with pagination
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>IPagedList of TEntity</returns>
        [CanBeNull]
        [System.Diagnostics.Contracts.Pure]
        IPagedList<TEntity> GetAll(int page, int pageSize=20);

        /// <summary>
        /// Returns a collection of all BaseEntity of TEntity in the DbSet without paginating
        /// </summary>
        /// <returns>IEnumerable of TEntity</returns>
        [CanBeNull]
        [System.Diagnostics.Contracts.Pure]
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Returns a collection of BaseEntity of TEntity that matches the supplied predicate with pagination.
        /// Throws ArgumentNullException if predicate is null.
        /// </summary>
        /// <param name="predicate">The condition</param>
        /// <returns>IEnumerable of TEntity</returns>
        [CanBeNull]
        [System.Diagnostics.Contracts.Pure]
        IEnumerable<TEntity> Where([NotNull]Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Returns a collection of BaseEntity of TEntity that matches the supplied predicate with pagination
        /// Throws ArgumentNullException if predicate is null.
        /// </summary>
        /// <param name="predicate">The condition</param>
        /// <param name="page">The current page</param>
        /// <param name="pageSize">The size of the page</param>
        /// <returns>IPagedList of TEntity</returns>
        [CanBeNull]
        [System.Diagnostics.Contracts.Pure]
        IPagedList<TEntity> Where([NotNull]Expression<Func<TEntity, bool>> predicate, int page, int pageSize = 20);

        /// <summary>
        /// Removes the supplied entity from the set
        /// </summary>
        /// <param name="entity">Entity to be remove</param>
        /// <returns>Nothing</returns>
        void Remove([NotNull]TEntity entity);

        /// <summary>
        /// Removes the entity with the supplied id
        /// </summary>
        /// <param name="id">the key of the entity to remove</param>
        /// <returns>Nothing</returns>
        void Remove<TKeyProperty>(TKeyProperty id);

        /// <summary>
        /// Removes the entities in the supplied collection.
        /// Throws ArgumentNullException if entities is null.
        /// </summary>
        /// <param name="entities">Entities to be removed</param>
        /// <returns>Nothing</returns>
        void RemoveRange([NotNull]IEnumerable<TEntity> entities);

        /// <summary>
        /// Adds the supplied entity to the set
        /// Throws ArgumentNullException if entity is null.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        void Add([NotNull]TEntity entity);

        /// <summary>
        /// Adds the supplied entity to the set
        /// Throws ArgumentNullException if entities is null.
        /// </summary>
        /// <param name="entities">List of entities to be added</param>
        void AddRange([NotNull]IEnumerable<TEntity> entities);

        /// <summary>
        /// Returns the number of entity that matches the supplied condition
        /// </summary>
        /// <param name="predicate">The condition to match</param>
        /// <returns>the number</returns>
        int Count(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Checks the entity in the expression.
        /// Throws ArgumentNullException if entity is null.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>true or false</returns>
        bool Contains([NotNull]TEntity entity);
        /// <summary>
        /// Checks the entity with the provided id
        /// </summary>
        /// <typeparam name="TKeyProperty">The primary key property</typeparam>
        /// <param name="id">The id of the entity</param>
        /// <returns>Boolean. true or false.</returns>
        bool Contains<TKeyProperty>(TKeyProperty id) ;

        /// <summary>
        /// Checks where there a match with the provided predicate.
        /// </summary>
        /// <param name="predicate">Search predicate.</param>
        /// <returns>Boolean. true or false.</returns>
        bool Contains(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// Updates an entity.
        /// Throws ArgumentNullException if entity is null.
        /// </summary>
        /// <param name="entity"></param>
        void Update([NotNull]TEntity entity);

        /// <summary>
        /// Returns a collection of BaseEntity of TEntity that matches the supplied predicate with pagination
        /// </summary>
        /// <param name="searchProperty">The search criteria</param>
        /// <param name="page">The current page</param>
        /// <param name="pageSize">The size of the page</param>
        /// <returns>IPagedList of TEntity</returns>
        [CanBeNull]
        [System.Diagnostics.Contracts.Pure]
        IPagedList<TEntity> Search(Expression<Func<TEntity, bool>>searchProperty, int page, int pageSize = 20);
        /// <summary>
        /// Returns a collection of BaseEntity of TEntity that matches the supplied predicate with pagination
        /// </summary>
        /// <param name="searchProperty">The search criteria</param>
        /// <returns>IPagedList of TEntity</returns>
        [CanBeNull]
        [System.Diagnostics.Contracts.Pure]
        IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> searchProperty);
    }
}