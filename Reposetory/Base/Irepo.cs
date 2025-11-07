namespace ShelfLife.Reposetory.Base
{
    public interface Irepo <T> where T : class
    {
        IEnumerable<T> GetAll();
        T FindById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        int Save();
    }
}
