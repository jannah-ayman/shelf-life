using Microsoft.EntityFrameworkCore;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Repository
{
    public class MainRepository<T> : Irepo<T> where T : class
    {
        protected readonly DBcontext _context;
        protected readonly DbSet<T> _set;

        public MainRepository(DBcontext context)
        {
            _context = context;
            _set = _context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _set.ToList();
        }

        public T FindById(int id)
        {
            return _set.Find(id);
        }

        public void Add(T entity)
        {
            _set.Add(entity);
        }

        public void Update(T entity)
        {
            _set.Update(entity);
        }

        public void Delete(T entity)
        {
            _set.Remove(entity);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}
