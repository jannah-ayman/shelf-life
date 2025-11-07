using Microsoft.EntityFrameworkCore;
using ShelfLife.Repository.Base;

namespace ShelfLife.Repository
{
    public class MainRepository<T> : Irepo<T> where T : class
    {
      
        protected DbContext _context;  
        public MainRepository(DbContext context)
        {
            _context = context;
        }
        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T FindById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Add(T entity)
        {
            _context.Add(entity);
        }

        public void Update(T entity)
        {
            _context.Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Remove(entity);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

    }
}
