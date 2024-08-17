using Microsoft.EntityFrameworkCore;
using Mshop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.DataAccess.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }


        public void Add(T entity)
        {
            _dbSet.Add(entity);

        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, string? includeWord = null, string includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);

            }
            if (includeWord != null)
            {
                foreach (var item in includeWord.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);


                }
            }
            return query.ToList();


        }

        public T GetFirstOrDefualt(Expression<Func<T, bool>>? predicate = null, string? includeWord = null)
        {
            IQueryable<T> query = _dbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);

            }
            if (includeWord != null)
            {
                foreach (var item in includeWord.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);


                }
            }
            return query.SingleOrDefault();

        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);

        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);

        }
    }
}
