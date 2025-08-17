using AgriFieldHub.Data;
using AgriFieldHub.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriFieldHub.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await Context.Set<User>()
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}
