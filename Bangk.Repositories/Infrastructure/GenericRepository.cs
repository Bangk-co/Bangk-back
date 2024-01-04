using Microsoft.EntityFrameworkCore;
using Bangk.Common;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bangk.Repositories.Infrastructure
{

    public class GenericRepository<TEntity, T> : IGenericRepository<TEntity, T> where TEntity : EntityBase
    {
        protected readonly AppDbContext Context;
        private readonly DbSet<TEntity> _dbSet;
        private bool _disposed;

        public GenericRepository(AppDbContext context)
        {
            Context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<List<TEntity>> GetAll(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = await GetAllQuery(filter, orderBy, includes);

            return query.ToList();
        }

        public Task<IQueryable<TEntity>> GetAllQuery(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return Task.FromResult(query);
        }

        public async Task<TEntity> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<TEntity> FirstOfDefaultAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.Where(filter);
            if (includes.Any())
            {
                foreach (Expression<Func<TEntity, object>> include in includes)
                    query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> Create(TEntity entity)
        {
            try
            {
                entity.CreationDate = DateTime.Now;

                await _dbSet.AddAsync(entity);
                return await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logs.WriteErrorLog(ex.Message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> CreateBulk(List<TEntity> entityList)
        {
            try
            {
                entityList.ForEach(entity =>
                {
                    entity.CreationDate = DateTime.Now;
                });

                await _dbSet.AddRangeAsync(entityList);
                return await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logs.WriteErrorLog(ex.Message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> UpdateBulk(List<TEntity> entityList)
        {
            try
            {
                _dbSet.UpdateRange(entityList);
                return await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logs.WriteErrorLog(ex.Message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Update(TEntity entity)
        {
            try
            {
                _dbSet.Update(entity);
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logs.WriteErrorLog(ex.Message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> Delete(TEntity entity)
        {
            try
            {
                _dbSet.Remove(entity);
                return await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logs.WriteErrorLog(ex.Message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> DeleteBulk(List<TEntity> entityList)
        {
            try
            {
                _dbSet.RemoveRange(entityList);
                return await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logs.WriteErrorLog(ex.Message, ex);
                throw new Exception(ex.Message);
            }
        }
        public int GetTotalDtoRecords(IQueryable<T> query)
        {
            return query.Count();
        }

        public async Task<IList<T>> GetDtoList(IQueryable<T> query, int pageSize, int currentPage)
        {
            if (pageSize > 0 && currentPage > 0)
            {
                query = query.Skip(pageSize * (currentPage - 1)).Take(pageSize);
            }

            return await query.ToListAsync();

        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                Context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}

