using AgriFieldHub.Data;
using AgriFieldHub.Models;

namespace AgriFieldHub.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IUserRepository Users { get; }
        public IRepository<Field> Fields { get; }
        public IRepository<Controller> Controllers { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Fields = new Repository<Field>(_context);
            Controllers = new Repository<Controller>(_context);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}
