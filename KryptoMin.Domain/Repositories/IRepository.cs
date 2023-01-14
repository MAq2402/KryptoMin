using KryptoMin.Domain.Entities;

namespace KryptoMin.Domain.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        Task<T> Get(Guid partitionKey, Guid rowKey);
        Task Add(T entity);
        Task Update(T entity);
    }
}