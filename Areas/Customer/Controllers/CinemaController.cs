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
        public IActionResult Index(int page=1)
        { 
            //var Cinemas = dbContext.Cinemas;
            var Cinemas = cinemaRepository.Get();
            //pagination
            var paginationPages = (int)Math.Ceiling((decimal)Cinemas.Count() / 7);
            if (page > paginationPages) page = paginationPages;
            Cinemas = Cinemas.Skip((page - 1) * 7).Take(7);
            ViewBag.paginationPages = paginationPages;
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
