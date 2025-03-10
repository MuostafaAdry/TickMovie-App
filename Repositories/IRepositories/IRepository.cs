using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query;

namespace MoviePoint.Repositories.IRepositories
{
    public interface IRepository<T> where T:class
    {
        public void Create(T entity)  ;
        public void Create(List<T> entities)  ;
        public void Edit(T entity)  ;
        public void Delete(T entity)  ;
        public void Delete(List<T> entities)  ;
        public void Commit() ;

        public IEnumerable<T> Get(
         Expression<Func<T, bool>>? filter = null,
        //Expression<IIncludableQueryable<T, object>>[]? thenincludes = null,
         Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProps = null,
        Expression<Func<T, object>>[]? includes = null,
        bool tracked = true);

        public T? GetOne(
            Expression<Func<T, bool>>? filter = null,
             //Expression<IIncludableQueryable<T, object>>[]? thenincludes = null,
             Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProps = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true);
    }
}
