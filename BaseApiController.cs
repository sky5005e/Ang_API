using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI.Data.GenericRepository;

namespace BaseSample.Controllers
{
   /// <summary>
   /// API Controller
   /// </summary>
   /// <typeparam name="TEntity"></typeparam>
   // [Authorize]
    public class BaseApiController<TEntity> : ApiController where TEntity : class
    {
        #region Properties.
        private GenericRepository<TEntity> _genericRepository;
        private WebAPI.Data.WebAPIEntities _context = new WebAPI.Data.WebAPIEntities();
        #endregion
        #region Public Constructor
        /// <summary>
        /// Public constructor to initialize patient service instance
        /// </summary>
        public BaseApiController()
        {
            _genericRepository = new GenericRepository<TEntity>(_context);
        }
        #endregion

      
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Get()
        {
            return _genericRepository.Get();
        }
       
        public virtual IHttpActionResult Get(int id)
        {
            var patient = _genericRepository.GetByID(id);
            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }
       
        public virtual IHttpActionResult Put(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _genericRepository.Update(entity);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

       
      
        public virtual IHttpActionResult Post(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _genericRepository.Insert(entity);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [Route("Filter")]
        public virtual IHttpActionResult Filter([FromUri]TEntity filter)
        {
            var properties = filter.GetType().GetProperties().Where(pi => pi.GetValue(filter) is string).Select(pi => new { Name = (string)pi.Name, Value = (string)pi.GetValue(filter) });
            if (properties.Count() > 0)
            {
                var param = Expression.Parameter(typeof(TEntity), "q");
                Expression filterExpression = null;
                foreach (var item in properties)
                {
                    var exp = Expression.Equal(Expression.Property(param, item.Name), Expression.Constant(item.Value));
                    if (filterExpression == null)
                        filterExpression = exp;
                    else
                        filterExpression = Expression.AndAlso(filterExpression, exp);
                }
                var lambda = Expression.Lambda<Func<TEntity, bool>>(filterExpression, param);
                var result = _genericRepository.GetManyQueryable(lambda);
                return Ok(result);
            }

            return Ok();
        }
        public Expression<Func<TEntity, bool>> GeneratorEqualityTest<TProperty>(Expression<Func<TEntity, TProperty>> accessor, TProperty expectedValue)
        {
            var body = Expression.Equal(accessor.Body, Expression.Constant(expectedValue));
            var predicate = Expression.Lambda<Func<TEntity, bool>>(body, accessor.Parameters);
            return predicate;
        }

        public virtual IHttpActionResult Delete(int id)
        {
            _genericRepository.Delete(id);
            _context.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

    }


   
}
