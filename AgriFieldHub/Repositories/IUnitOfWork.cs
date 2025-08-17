using AgriFieldHub.Models;

namespace AgriFieldHub.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRepository<Field> Fields { get; }
        IRepository<Controller> Controllers { get; }
        Task<int> SaveChangesAsync();
    }
}
