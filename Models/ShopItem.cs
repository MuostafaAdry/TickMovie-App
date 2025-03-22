using Stripe.Climate;

namespace MoviePoint.Models
{
    public class ShopItem
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public Shop Shop { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
