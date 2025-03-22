using E_commerce2.Models;
using MoviePoint.DataAccess;
using MoviePoint.Repositories.IRepositories;
using MoviePoint.Repository;

namespace MoviePoint.Repositories
{
    public class CartRepository:Repository<Cart>,ICartRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CartRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}
