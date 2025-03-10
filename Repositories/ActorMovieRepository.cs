using MoviePoint.DataAccess;
using MoviePoint.Models;
using MoviePoint.Repositories.IRepositories;
using MoviePoint.Repository;

namespace MoviePoint.Repositories
{
    public class ActorMovieRepository :Repository<ActorMovie>,IActorMovieRepositories
    {
        private readonly ApplicationDbContext dbContext;
        public ActorMovieRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
