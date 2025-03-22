using E_commerce2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviePoint.Models;
using MoviePoint.Models.ViewModel;




namespace MoviePoint.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ActorMovie> ActorMovies { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<ShopItem> ShopItems { get; set; }

        //public DbSet<Order> Orderss { get; set; }





        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
        }
        public ApplicationDbContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Movies;Integrated Security=True;TrustServerCertificate=True\r\n");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

             
        }
        public DbSet<MoviePoint.Models.ViewModel.RegisterVM> RegisterVM { get; set; } = default!;
        public DbSet<MoviePoint.Models.ViewModel.LoginVM> LoginVM { get; set; } = default!;


    }
}
