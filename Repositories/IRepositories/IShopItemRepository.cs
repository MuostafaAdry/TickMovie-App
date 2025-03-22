using MoviePoint.Models;

namespace MoviePoint.Repositories.IRepositories
{
    public interface IShopItemRepository:IRepository<ShopItem>
    {
        public void CreateRange(IEnumerable<ShopItem> shopItem);
    }
}
