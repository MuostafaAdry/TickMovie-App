 using Microsoft.AspNetCore.Authorization;
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
    //[Authorize(Roles = "Customer")]
    public class HomeController : Controller
    {
         ApplicationDbContext dbContext = new ApplicationDbContext();

       
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
        public IActionResult NotFoundPage()
        {
            return View();
        }

        public IActionResult Index(string? categoryName,string? movieName,int page=1)
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
            //pagination
            var paginationPages = (int)Math.Ceiling((decimal)Movies.Count()/8);
            if (page > paginationPages) page = paginationPages;
            Movies = Movies.Skip((page - 1) * 8).Take(8);
            ViewBag.paginationPages = paginationPages;
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

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
