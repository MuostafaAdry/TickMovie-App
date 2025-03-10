using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MoviePoint.DataAccess;
using MoviePoint.Models;
using MoviePoint.Repositories;
using MoviePoint.Repositories.IRepositories;
using System.Linq.Expressions;

namespace MoviePoint.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
         ApplicationDbContext dbContext = new ApplicationDbContext();

        //MovieRepository movieRepository = new MovieRepository();
        //CategoryRepository categoryRepository = new CategoryRepository();
        //ActorMovieRepository actorMovieRepository = new ActorMovieRepository();
        private readonly IMovieRepositories movieRepository;
        private readonly ICategoryRepositories categoryRepository;
        private readonly IActorRepositories actorRepository;
    

        public HomeController(
            IMovieRepositories movieRepository,
            ICategoryRepositories categoryRepository,
            IActorRepositories actorRepository)
        {
            this.movieRepository = movieRepository;
            this.categoryRepository = categoryRepository;
            this.actorRepository = actorRepository;
        }

        public IActionResult Index(string? categoryName,string? movieName)
        {
            //IQueryable<Movie> Movies = dbContext.Movies.Include(e => e.Cinema).Include(e => e.Category);
            //var Movies = movieRepository.Get(includes: [e => e.Cinema, em => em.Category]);
            var Movies = movieRepository.Get(includeProps: e => e.Include(e => e.Cinema).Include(e => e.Category));

            if (categoryName != null)
            {
                Movies=Movies.Where(e => e.Category.Name == categoryName);
            }
            if (movieName!=null)
            {
                Movies=Movies.Where(e => e.Name.Contains(movieName));
            }
            //filter  with category
            //ViewBag.Categories = dbContext.Categories.ToList();
            ViewBag.Categories = categoryRepository.Get();
           
            return View(Movies.ToList());
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult MoreDetails(int movieId)
        {


            //ViewBag.ActorMovie = dbContext.ActorMovies.Where(e => e.MovieId == movieId);

            //ViewBag.ActorMovies = actorMovieRepository.Get(e => e.MovieId == movieId);


            //var movieDetails = dbContext.Movies
            //    .Include(e => e.Category)
            //    .Include(e => e.Cinema)
            //     .Include(e => e.ActorMovies)
            //     .ThenInclude(e => e.Actor)
            //    .FirstOrDefault(e => e.Id == movieId);



            var movieDetails = movieRepository.GetOne(
                filter: e => e.Id == movieId,
                includeProps: q => q.Include(e => e.Category)
                                   .Include(e => e.Cinema)
                                   .Include(e => e.ActorMovies)
                                   .ThenInclude(e => e.Actor));




            if (movieDetails != null)
            {
                return View(movieDetails);
            }
            return View("Index");
        }
        //all movies related with actor
        public IActionResult ActorDetails(int actorId)
        {
             
            //var actor = dbContext.Actors
            //    .Include(e => e.ActorMovies)  
            //    .ThenInclude(e => e.Movie)
            //    .ThenInclude(e => e.Category)

            //   //هنه الازم اعمل نفس الخطوات ال فوق لام اجيب السينما علاشن الموفى هوه الفيه السينما وليس الكاتيجورى   

            //   .Include(e => e.ActorMovies)
            //   .ThenInclude(e => e.Movie)
            //   .ThenInclude(e => e.Cinema)

           

            //   .FirstOrDefault(e => e.Id == actorId); 
            
            var actor = actorRepository.GetOne(
        filter: e => e.Id == actorId,
        includeProps: q => q.Include(e => e.ActorMovies)
                           .ThenInclude(am => am.Movie)
                           .ThenInclude(m => m.Category)

                           .Include(e => e.ActorMovies)
                           .ThenInclude(am => am.Movie)
                           .ThenInclude(m => m.Cinema));
            if (actor != null)
            {
                return View(actor);
            }
            return View("Index");
        }
    }
}
