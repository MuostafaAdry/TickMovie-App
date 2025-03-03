using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.DataAccess;
using MoviePoint.Models;

namespace MoviePoint.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();

        public IActionResult Index(string? categoryName,string? movieName)
        {
            IQueryable<Movie> Movies = dbContext.Movies.Include(e => e.Cinema).Include(e => e.Category);

            if (categoryName != null)
            {
                Movies=Movies.Where(e => e.Category.Name == categoryName);
            }

             
            if (movieName!=null)
            {
                Movies=Movies.Where(e => e.Name.Contains(movieName));
            }
            //filter  with category
            ViewBag.Categories = dbContext.Categories.ToList();
            return View(Movies.ToList());
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult MoreDetails(int movieId)
        {
            //ViewBag.Categories = dbContext.Categories.ToList();

            //ViewBag.ActorMovies = dbContext.ActorMovies.Where(e => e.MovieId == movieId);

            var movieDetails = dbContext.Movies
                .Include(e => e.Category)
                .Include(e => e.Cinema)
                 .Include(e => e.ActorMovies)
                 .ThenInclude(e => e.Actor)
                .FirstOrDefault(e => e.Id == movieId);

            if (movieDetails != null)
            {
                return View(movieDetails);
            }
            return View("Index");
        }
        //all movies related with actor
        public IActionResult ActorDetails(int actorId)
        {
            ViewBag.Categories = dbContext.Categories.ToList();
            var actor = dbContext.Actors
                .Include(e => e.ActorMovies)  
                .ThenInclude(e => e.Movie)
                .ThenInclude(e => e.Category)

               //هنه الازم اعمل نفس الخطوات ال فوق لام اجيب السينما علاشن الموفى هوه الفيه السينما وليس الكاتيجورى   

               .Include(e => e.ActorMovies)
               .ThenInclude(e => e.Movie)
               .ThenInclude(e => e.Cinema)
               .FirstOrDefault(e => e.Id == actorId);
            if (actor != null)
            {
                return View(actor);
            }
            return View("Index");
        }
    }
}
