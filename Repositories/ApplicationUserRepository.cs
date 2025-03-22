using MoviePoint.DataAccess;
using MoviePoint.Models;
using MoviePoint.Repositories.IRepositories;
using MoviePoint.Repository;

namespace MoviePoint.Repositories
{
    public class ApplicationUserRepository:Repository<ApplicationUser>,IApplicationUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ApplicationUserRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}
