using Bangk.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bangk.Repositories.Infrastructure
{
    public interface IGenericRepository<TEntity, T> where TEntity : EntityBase
    {
        Task<List<TEntity>> GetAll(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includes);

        Task<IQueryable<TEntity>> GetAllQuery(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> GetById(int id);

        Task<TEntity> FirstOfDefaultAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes);

        Task<int> Create(TEntity entity);

        Task<int> CreateBulk(List<TEntity> entityList);

        Task<bool> Update(TEntity entity);

        Task<int> Delete(TEntity entity);

        int GetTotalDtoRecords(IQueryable<T> query);

        Task<IList<T>> GetDtoList(IQueryable<T> query, int pageSize, int currentPage);

        Task<int> UpdateBulk(List<TEntity> entityList);

        Task<int> DeleteBulk(List<TEntity> entityList);
    }
}
