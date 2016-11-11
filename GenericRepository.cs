#region Using Namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;


#endregion

namespace Intrepid.Accent.Data
{
   
    /// <summary>
    /// Generic Repository class for Entity Operations
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class GenericRepository<TEntity> where TEntity : EntityObject
    {
        #region Private member variables...
        internal IntrepidAccentEntities Context;
        internal ObjectSet<TEntity> ObjectSet;
        #endregion

        #region Public Constructor...
        /// <summary>
        /// Public Constructor,initializes privately declared local variables.
        /// </summary>
        /// <param name="context"></param>
        public GenericRepository(IntrepidAccentEntities context)
        {
            this.Context = context;
            this.ObjectSet = Context.CreateObjectSet<TEntity>();
        }
        #endregion

        #region Public member methods...

        /// <summary>
        /// Generic get method on the basis of id for Entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetByID(object id)
        {
            var keyPropertyName = ObjectSet.EntitySet.ElementType.KeyMembers[0].ToString();
            return  ObjectSet.Where("it." + keyPropertyName + "=" + id).First();
            //return ObjectSet.SingleOrDefault(t => t.EntityKey.EntityKeyValues[0].Value == id);
        }

        /// <summary>
        /// generic Insert method for the entities
        /// </summary>
        /// <param name="entity"></param>
        public virtual bool Insert(TEntity entity)
        {
            Context.AttachTo(entity.GetType().Name, entity);
            Context.ObjectStateManager.ChangeObjectState(entity, EntityState.Added);
            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// Generic Delete method for the entities
        /// </summary>
        /// <param name="id"></param>
        public virtual bool Delete(object id)
        {
            TEntity entityToDelete = ObjectSet.SingleOrDefault(t => t.EntityKey.EntityKeyValues[0].Value == id);
            if (entityToDelete.EntityState == EntityState.Detached)
            {
                Context.Attach(entityToDelete);
            }
            Context.DeleteObject(entityToDelete);
            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// Generic Delete method for the entities
        /// </summary>
        /// <param name="entityToDelete"></param>
        public virtual bool Delete(TEntity entityToDelete)
        {
            if (entityToDelete.EntityState == EntityState.Detached)
            {
                Context.Attach(entityToDelete);
            }
            Context.DeleteObject(entityToDelete);
            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// Generic update method for the entities
        /// </summary>
        /// <param name="entityToUpdate"></param>
        public virtual bool Update(TEntity entityToUpdate)
        {
            Context.ObjectStateManager.ChangeObjectState(entityToUpdate, EntityState.Modified);
            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// generic method to get many record on the basis of a condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetMany(Func<TEntity, bool> where)
        {
            return ObjectSet.Where(where).ToList();
        }

        /// <summary>
        /// generic method to get many record on the basis of a condition but query able.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetManyQueryable(Func<TEntity, bool> where)
        {
            return ObjectSet.Where(where).AsQueryable();
        }


        ///// <summary>
        ///// generic method to get many record on the basis of a condition but query able.
        ///// </summary>
        ///// <param name="where"></param>
        ///// <returns></returns>
        //public virtual IQueryable<TEntity> GetManyQueryable(Expression<Func<TEntity, bool>> where)
        //{
        //    return DbSet.Where(where).AsQueryable();
        //}
        /// <summary>
        /// generic get method , fetches data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public TEntity Get(Func<TEntity, Boolean> where)
        {
            return ObjectSet.Where(where).FirstOrDefault<TEntity>();
        }

        /// <summary>
        /// generic delete method , deletes data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool Delete(Func<TEntity, Boolean> where)
        {
            IQueryable<TEntity> objects = ObjectSet.Where(where).AsQueryable();
            foreach (TEntity obj in objects)
                Context.DeleteObject(obj);
            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// generic method to fetch all the records from db
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            return ObjectSet.ToList();
        }

        /// <summary>
        /// Inclue multiple
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetWithInclude(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate, params string[] include)
        {
            IQueryable<TEntity> query = this.ObjectSet;
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
            return query.Where(predicate);
        }

        /// <summary>
        /// Generic method to check if entity exists
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public bool Exists(object primaryKey)
        {
            return ObjectSet.FirstOrDefault(t => t.EntityKey.EntityKeyValues[0].Value == primaryKey) != null;
        }

        /// <summary>
        /// Gets a single record by the specified criteria (usually the unique identifier)
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record that matches the specified criteria</returns>
        public TEntity GetSingle(Func<TEntity, bool> predicate)
        {
            return ObjectSet.Single<TEntity>(predicate);
        }

        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        public TEntity GetFirst(Func<TEntity, bool> predicate)
        {
            return ObjectSet.First<TEntity>(predicate);
        }


        #endregion
    }
}