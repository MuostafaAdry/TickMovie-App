using E_commerce2.Models;
using MoviePoint.DataAccess;
using MoviePoint.Models;
using MoviePoint.Repositories.IRepositories;
using MoviePoint.Repository;

namespace MoviePoint.Repositories
{
    public class ShopRepository: Repository<Shop>, IShopRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ShopRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}
