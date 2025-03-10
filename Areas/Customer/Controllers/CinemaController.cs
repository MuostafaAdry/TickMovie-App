using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.DataAccess;
using MoviePoint.Repositories;
using MoviePoint.Repositories.IRepositories;

namespace MoviePoint.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CinemaController : Controller
    {
        //ApplicationDbContext dbContext = new ApplicationDbContext();
        private readonly ICinemaRepositories cinemaRepository ;
        private readonly IMovieRepositories movieRepository ;

        public CinemaController(ICinemaRepositories cinemaRepository, IMovieRepositories movieRepository)
        {
            this.cinemaRepository = cinemaRepository;
            this.movieRepository = movieRepository;
        }
        public IActionResult Index()
        { 
            //var Cinemas = dbContext.Cinemas;
            var Cinemas = cinemaRepository.Get();
            return View(Cinemas.ToList());
        }

        public IActionResult ShowMovies(int cinemaId)
        {
             //var CinemaMovies = dbContext.Movies.Include(e => e.Category).Include(e => e.Cinema)
             //   .Where(e => e.CinemaId == cinemaId); 
            
            var CinemaMovies = movieRepository.Get(includes: [e => e.Category,e => e.Cinema])
                .Where(e => e.CinemaId == cinemaId);

            return View(CinemaMovies.ToList());
        }
    }

    
}
