using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UdemyCore.Repositories;

namespace UdemyData.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {

        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>(); 
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity); // state Add olarak memory'de işlenecek
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id); // aynı anda 2 alan PK olarak işaretlenebilir o yüzden findAsync params alır

            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Detached; // bu entity memory'de takip edilmesin.
            }

            return entity;
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity); // entitystate remove yapıldı
        }

        public T Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified; // tüm alanlar güncellenir. mesala sade name güncelledin price'de stock'da güncellenir.
                                                                 // bu da performans kaybı yaşatır. genericrepository'lerin zararlarından biri          // entity.firstordefault(x=>x.id == entity.id); olması gerekirdi sonra savechanges() 
            return entity;
        }

        /// <summary>
        /// ToList dediğimiz zaman veritabanına yansıyor.
        /// IEnumarable yaparsak veriyi memory'e çeker.
        /// IQueryable ile yapacağımız sorgular db'de gerçekleşir where order by gibi en son toList denince veri tek seferde db'den çekilir.
        /// </summary>
        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
    }
}
