using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoviePoint.DataAccess;
using MoviePoint.Models;
using MoviePoint.Repositories;
using MoviePoint.Repositories.IRepositories;
using MoviePoint.Utility;
using Stripe;

namespace MoviePoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            //inject of identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option => {
                option.SignIn.RequireConfirmedEmail = false;
            })
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();

            // Configure application cookie
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Register";
                options.AccessDeniedPath = "/Customer/Home/AccessDenied"; // Redirect unauthorized users
            });

            builder.Services.AddScoped<IActorRepositories, ActorRepository>();
            builder.Services.AddScoped<IMovieRepositories, MovieRepository>();
            builder.Services.AddScoped<ICategoryRepositories, CategoryRepository>();
            builder.Services.AddScoped<ICinemaRepositories, CinemaRepository>();
            builder.Services.AddScoped<IActorMovieRepositories, ActorMovieRepository>();
            builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<IShopRepository, ShopRepository>();
            builder.Services.AddScoped<IShopItemRepository, ShopItemRepository>();
            // Configure Stripe settings
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

          
            app.UseRouting();
           


            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
