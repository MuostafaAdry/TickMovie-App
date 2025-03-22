using Microsoft.EntityFrameworkCore;
using MoviePoint.Models;

namespace E_commerce2.Models
{
    [PrimaryKey(nameof(MovieId), nameof(ApplicationUserId))]
    public class Cart
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int Count { get; set; }
    }
}
