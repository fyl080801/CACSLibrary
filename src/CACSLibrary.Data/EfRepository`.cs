using CACSLibrary.Component;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;

namespace CACSLibrary.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EfRepository<T> : IRepository<T> where T : BaseObjectEntity
    {
        static readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
        readonly IDbContext _context;
        IDbSet<T> _entities;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public EfRepository(IDbContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get
            {
                IQueryable<T> entities;
                using (new WriteLocker(EfRepository<T>._locker))
                {
                    entities = this.Entities;
                }
                return entities;
            }
        }

        private IDbSet<T> Entities
        {
            get
            {
                if (this._entities == null)
                {
                    this._entities = this._context.Set<T>();
                }
                return this._entities;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                this.Entities.Add(entity);
                using (new WriteLocker(EfRepository<T>._locker))
                {
                    this._context.SaveChanges();
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                string msg = string.Empty;
                foreach (DbEntityValidationResult validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (DbValidationError validationError in validationErrors.ValidationErrors)
                    {
                        msg = msg + string.Format(" Ù–‘: {0} ¥ÌŒÛ: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                Exception fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                string msg = string.Empty;
                foreach (DbEntityValidationResult validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (DbValidationError validationError in validationErrors.ValidationErrors)
                    {
                        msg = msg + Environment.NewLine + string.Format(" Ù–‘: {0} ¥ÌŒÛ: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                Exception fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                this.Entities.Remove(entity);
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                string msg = string.Empty;
                foreach (DbEntityValidationResult validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (DbValidationError validationError in validationErrors.ValidationErrors)
                    {
                        msg = msg + Environment.NewLine + string.Format(" Ù–‘: {0} ¥ÌŒÛ: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                Exception fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void Insert(params T[] entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entity");
                for (int i = 0; i < entities.Length; i++)
                {
                    T entity = entities[i];
                    this.Entities.Add(entity);
                }
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                string msg = string.Empty;
                foreach (DbEntityValidationResult validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (DbValidationError validationError in validationErrors.ValidationErrors)
                    {
                        msg = msg + string.Format(" Ù–‘: {0} ¥ÌŒÛ: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                Exception fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void Update(params T[] entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");
            if (entities.Length <= 0)
                throw new ArgumentException("entities");
            this.Update(entities[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void Delete(params T[] entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");
                for (int i = 0; i < entities.Length; i++)
                {
                    T entity = entities[i];
                    this.Entities.Remove(entity);
                }
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                string msg = string.Empty;
                foreach (DbEntityValidationResult validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (DbValidationError validationError in validationErrors.ValidationErrors)
                    {
                        msg = msg + Environment.NewLine + string.Format(" Ù–‘: {0} ¥ÌŒÛ: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                Exception fail = new Exception(msg, dbEx);
                throw fail;
            }
        }
    }
}
