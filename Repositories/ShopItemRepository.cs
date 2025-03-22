using MoviePoint.DataAccess;
using MoviePoint.Models;
using MoviePoint.Repositories.IRepositories;
using MoviePoint.Repository;

namespace MoviePoint.Repositories
{
    public class ShopItemRepository : Repository<ShopItem>,IShopItemRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ShopItemRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public void CreateRange(IEnumerable<ShopItem> shopItem)
        {
            _dbContext.AddRange(shopItem);
        }
    }
}
