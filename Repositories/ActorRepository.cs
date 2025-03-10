using MoviePoint.DataAccess;
using MoviePoint.Models;
using MoviePoint.Repositories.IRepositories;
using MoviePoint.Repository;

namespace MoviePoint.Repositories
{
    public class ActorRepository :Repository<Actor>,IActorRepositories
    {
        private readonly ApplicationDbContext dbContext;
        public ActorRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
