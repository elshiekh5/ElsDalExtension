using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace BigDemo.Repasitory
{
    public class GenericRepository<TEntity>  where TEntity : class
    {
        private readonly DbContext context;
        private DbSet<TEntity> Entities;
        string errorMessage = string.Empty;
        #region ----------------Constructor----------------
        //---------------------------------------------
        //Constructor
        //---------------------------------------------
        public GenericRepository(DbContext context)
        {
            this.context = context;
            this.Entities = context.Set<TEntity>();
        }
        //---------------------------------------------
        #endregion

        #region ----------------GetDbSet----------------
        //---------------------------------------------
        //GetAll
        //---------------------------------------------
        public DbSet<TEntity> GetDbSet()
        {
            return Entities;
        }
        //---------------------------------------------
        #endregion

        #region ----------------GetAll----------------
        //---------------------------------------------
        //GetAll
        //---------------------------------------------
        public IQueryable<TEntity> GetAll()
        {
            return Entities;
        }
        //---------------------------------------------
        #endregion

        #region ----------------GetByKey----------------
        //---------------------------------------------
        //GetByKey
        //---------------------------------------------
        public TEntity GetByKey(object id)
        {
            return Entities.Find(id);
        }
        //---------------------------------------------
        #endregion

        #region ----------------Delete----------------
        //---------------------------------------------
        //Delete Action
        //Get: /News/Delete/5
        //---------------------------------------------
        public void Delete(object id)
        {
            TEntity entity = Entities.Find(id);
            Entities.Remove(entity);
            /*bool result = false;
            try
            {
                TEntity entity = Entities.Find(id);
                Entities.Remove(entity);
                context.Commit();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;*/
        }
        //---------------------------------------------
        #endregion

        #region ----------------Add----------------
        //---------------------------------------------
        //Delete Action
        //Get: /News/Delete/5
        //---------------------------------------------
        public void Add(TEntity entity)
        {
                Entities.Add(entity);
            /*bool result = false;
            try
            {
                Entities.Add(entity);
                context.Commit();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;*/
        }
        //---------------------------------------------
        #endregion

        #region ----------------Update----------------
        //---------------------------------------------
        //Delete Action
        //Get: /News/Delete/5
        //---------------------------------------------
        public void Update(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;

            /*
            bool result = false;
            try
            {
                context.Entry(entity).State = EntityState.Modified;
                context.Commit();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;*/
        }
        //---------------------------------------------
        #endregion

        #region ----------------SetModify----------------
        //---------------------------------------------
        //SetModify
        //---------------------------------------------
        public void SetModify(TEntity entity)
        {
            //If object with the same id in the OSM then should be detached
            context.Entry(entity).State = EntityState.Modified;
        }
        //---------------------------------------------
        #endregion

        #region ----------------SetRemoved----------------
        //---------------------------------------------
        //SetRemoved
        //---------------------------------------------
        public void SetRemoved(TEntity entity)
        {
            //If object with the same id in the OSM then should be detached
            context.Entry(entity).State = EntityState.Deleted;
        }
        //---------------------------------------------
        #endregion

        #region -------------------GetList----------------
        //------------------------------------------------
        //GetList
        //------------------------------------------------
        //Examples 
        /*
         //with predicate && Selector && count
            return this.GetList((e => e.UserID == userId && e.IsDefault == true && e.IsActive == true), 
                e => new Entities.UserAddress
                {
                    UserID = e.UserID,
                    FName = e.FName,
                    LName = e.LName,
                    Street = e.Street
                },
                10
                );
        //--------------------------
         //with predicate only
            return this.GetList((e => e.UserID == userId && e.IsDefault == true && e.IsActive == true), null);
        //--------------------------
         //with selector only
            return this.GetList(null,e => new Entities.UserAddress
                {
                    UserID = e.UserID,
                    FName = e.FName,
                    LName = e.LName,
                    Street = e.Street
                });
        //--------------------------
         //without predicate and selector or count
            return this.GetList(null,null);
        //--------------------------
        */
        //------------------------------------------------
        public List<TEntity> GetList(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TEntity>> selector,
            int count = -1)
        {
            var query = Entities
             .Where(predicate ?? (c => true));

            //count
            if (count > 0)
            {
                query = query.Take(count);
            }
            //selector
            if (selector != null)
            {
                query.Select(selector);
            }
           //return the list result
            return query.ToList();
        }

        //------------------------------------------------
        #endregion

        #region -------------------GetListUsingFunc----------------
        //------------------------------------------------
        //GetListUsingFunc
        //------------------------------------------------
        //Examples 
        /*
         //with predicate && Selector && count
            return this.GetListUsingFunc((e => e.UserID == userId && e.IsDefault == true && e.IsActive == true), 
                e => new Entities.UserAddress
                {
                    UserID = e.UserID,
                    FName = e.FName,
                    LName = e.LName,
                    Street = e.Street
                },
                10
                );
        //--------------------------
         //with predicate only
            return this.GetListUsingFunc((e => e.UserID == userId && e.IsDefault == true && e.IsActive == true), null);
        //--------------------------
         //with selector only
            return this.GetListUsingFunc(null,e => new Entities.UserAddress
                {
                    UserID = e.UserID,
                    FName = e.FName,
                    LName = e.LName,
                    Street = e.Street
                });
        //--------------------------
         //without predicate and selector or count
            return this.GetListUsingFunc(null,null);
        //--------------------------
        */
        //------------------------------------------------
        public List<TEntity> GetListUsingFunc(
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity, TEntity> selector,
            int count = -1){

            var query = Entities
                 .Where(predicate ?? (c => true));
            //count
            if (count > 0)
            {

                query = query.Take(count);
            }
            //selector
            if (selector != null)
            {
                query.Select(selector);
            }
            
            return query.ToList();

        }

        //------------------------------------------------
        #endregion

        #region -------------------GetListDynamicProps----------------
        //------------------------------------------------
        //GetListDynamicProps
        //------------------------------------------------
        //Examples 
        /*
         //with predicate && Selector props
            return this.GetListDynamicProps(
                (e => e.UserID == userId && e.IsDefault == true && e.IsActive == true), 
                "UserID,FName,LName,Street"
                );
        //--------------------------
         //with predicate only
            return this.GetListDynamicProps((e => e.UserID == userId && e.IsDefault == true && e.IsActive == true), null);
        //--------------------------
         //with selector props only
            return this.GetListDynamicProps(null,"UserID,FName,LName,Street");
        //--------------------------
         //without predicate and selector 
            return this.GetListDynamicProps(null,null);
        //--------------------------
        */
        //------------------------------------------------

        public List<TEntity> GetListDynamicProps(
            Expression<Func<TEntity, bool>> predicate,
            string props,
            int count = -1)
        {
            var selector = CreateSelectorForAdynamicEfQuery(props);
            return GetListUsingFunc(predicate, selector, count);
        }

        //------------------------------------------------
        #endregion

        #region -------------------GetOne----------------
        //------------------------------------------------
        //GetOne
        //------------------------------------------------
        //Examples 
        /*
         //with predicate && Selector
            return this.GetOne((e => e.UserID == userId && e.IsDefault == true && e.IsActive == true), 
                e => new Entities.UserAddress
                {
                    UserID = e.UserID,
                    FName = e.FName,
                    LName = e.LName,
                    Street = e.Street
                }
                );
        //--------------------------
         //with predicate only
            return this.GetOne((e => e.UserID == userId && e.IsDefault == true && e.IsActive == true), null);
        //--------------------------
         //with selector only
            return this.GetOne(null,e => new Entities.UserAddress
                {
                    UserID = e.UserID,
                    FName = e.FName,
                    LName = e.LName,
                    Street = e.Street
                });
        //--------------------------
         //without predicate and selector 
            return this.GetOne(null,null);
        //--------------------------
        */
        //------------------------------------------------
        public TEntity GetOne(
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity, TEntity> selector)
        {
            var query = Entities
             .Where(predicate ?? (c => true));
            if (selector != null)
            {
                query.Select(selector);
            }
            return query.SingleOrDefault();
        }

        //------------------------------------------------
        #endregion

        #region ----------------CreateSelectorForAdynamicEfQuery----------------
        //---------------------------------------------
        //CreateSelectorForAdynamicEfQuery
        //---------------------------------------------
        /// <summary>
        /// create a selector to use for a dynamic entity framework query 
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        /// <example>
        ///     var result = list.Select( CreateSelectorForAdynamicEfQuery( "Field1, Field2" ) );
        /// </example>
        private Func<TEntity, TEntity> CreateSelectorForAdynamicEfQuery(string fields)
        {
            // input parameter "o"
            var xParameter = Expression.Parameter(typeof(TEntity), "o");

            // new statement "new Data()"
            var xNew = Expression.New(typeof(TEntity));

            // create initializers
            var bindings = fields.Split(',').Select(o => o.Trim())
                .Select(o =>
                {

                    // property "Field1"
                    var mi = typeof(TEntity).GetProperty(o);

                    // original value "o.Field1"
                    var xOriginal = Expression.Property(xParameter, mi);

                    // set value "Field1 = o.Field1"
                    return Expression.Bind(mi, xOriginal);
                }
            );

            // initialization "new Data { Field1 = o.Field1, Field2 = o.Field2 }"
            var xInit = Expression.MemberInit(xNew, bindings);

            // expression "o => new Data { Field1 = o.Field1, Field2 = o.Field2 }"
            var lambda = Expression.Lambda<Func<TEntity, TEntity>>(xInit, xParameter);

            // compile to Func<Data, Data>
            return lambda.Compile();
        }
        //---------------------------------------------
        #endregion
    }
}
