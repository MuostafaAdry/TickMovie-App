using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.DataAccess;

namespace MoviePoint.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CinemaController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        public IActionResult Index()
        {
            //ViewBag.Categories = dbContext.Categories.ToList();
            var Cinemas = dbContext.Cinemas;
            return View(Cinemas.ToList());
        }

        public IActionResult ShowMovies(int cinemaId)
        {
            //ViewBag.Categories = dbContext.Categories.ToList();
            var CinemaMovies = dbContext.Movies.Include(e => e.Category).Include(e => e.Cinema)
                .Where(e => e.CinemaId == cinemaId).ToList();
            return View(CinemaMovies);
        }
    }

    
}
