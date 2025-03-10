using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.DataAccess;
using MoviePoint.Models;
using MoviePoint.Repositories;
using MoviePoint.Repositories.IRepositories;

namespace MoviePoint.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        //ApplicationDbContext dbContext = new ApplicationDbContext();

        //MovieRepository movieRepository = new MovieRepository();
        //CategoryRepository categoryRepository = new CategoryRepository();
        //CinemaRepository cinemaRepository = new CinemaRepository();
        //ActorRepository actorRepository = new ActorRepository();
        //ActorMovieRepository actorMovieRepository = new ActorMovieRepository();

        private readonly IMovieRepositories movieRepository;
        private readonly ICategoryRepositories categoryRepository;
        private readonly ICinemaRepositories cinemaRepository;
        private readonly IActorRepositories actorRepository;
        private readonly IActorMovieRepositories actorMovieRepository;
        public MovieController(
             IMovieRepositories movieRepository,
             ICategoryRepositories categoryRepository,
             ICinemaRepositories cinemaRepository,
             IActorRepositories actorRepository,
             IActorMovieRepositories actorMovieRepository
            )
        {
            this.movieRepository = movieRepository;
            this.categoryRepository = categoryRepository;
            this.cinemaRepository = cinemaRepository;
            this.actorRepository = actorRepository;
            this.actorMovieRepository = actorMovieRepository;
        }
        public IActionResult Index()
        {
            //var movies = dbContext.Movies.Include(e=>e.Category).Include(e=>e.Cinema);
            var movies = movieRepository.Get(includes: [e => e.Category, e => e.Cinema]);


            return View(movies.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.movieCategory = categoryRepository.Get().ToList();
            ViewBag.movieCinema = cinemaRepository.Get().ToList();
            ViewBag.actor = actorRepository.Get().ToList();

            return View(new Movie());
        }




        [HttpPost]
        public IActionResult Create(Movie movie, IFormFile movieFile, List<int> moviesId)
        {
            if (ModelState.IsValid)
            {

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(movieFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\movies", fileName);
                //copy img in the wwwroot
                using (var stream = System.IO.File.Create(filePath))
                {
                    movieFile.CopyTo(stream);
                }
                //save img path in db

                movie.ImgUrl = fileName;
                //dbContext.Movies.Add(movie);
                movieRepository.Create(movie);
                movieRepository.Commit();
                //  add actor ids 
                if (moviesId != null)
                {
                    foreach (var actorId in moviesId)
                    {
                        var movieActor = new ActorMovie
                        {
                            MovieId = movie.Id,
                            ActorId = actorId
                        };
                        //dbContext.ActorMovies.Add(movieActor);
                        actorMovieRepository.Create(movieActor);
                    }
                    actorMovieRepository.Commit();
                }
                return RedirectToAction("Index");
            }
            //ViewBag.actor = dbContext.Actors.ToList();
            //ViewBag.movieCategory = dbContext.Categories.ToList();
            //ViewBag.movieCinema = dbContext.Cinemas.ToList();
            ViewBag.movieCategory = categoryRepository.Get().ToList();
            ViewBag.movieCinema = cinemaRepository.Get().ToList();
            ViewBag.actor = actorRepository.Get().ToList();
            return View(movie);
        }
        public IActionResult Edit(int movieId)
        {
            //ViewBag.actor = dbContext.Actors.ToList();
            //ViewBag.movieCategory = dbContext.Categories.ToList();
            //ViewBag.movieCinema = dbContext.Cinemas.ToList();
            ViewBag.movieCategory = categoryRepository.Get().ToList();
            ViewBag.movieCinema = cinemaRepository.Get().ToList();
            ViewBag.actor = actorRepository.Get().ToList();
            //var movie = dbContext.Movies.FirstOrDefault(e => e.Id == movieId);
            var movie = movieRepository.GetOne(e => e.Id == movieId);
            if (movie != null)
            {
                return View(movie);
            }
            return RedirectToAction("NotFoundPage");
        }
        [HttpPost]
        public IActionResult Edit(Movie movie, IFormFile movieFile, List<int> moviesId)
        {
            if (movie != null)
            {
                //var OldFileInWWWRoot = dbContext.Movies.AsNoTracking().FirstOrDefault(e => e.Id == movie.Id).ImgUrl;
                var OldFileInWWWRoot = movieRepository.GetOne(e => e.Id == movie.Id, tracked: false).ImgUrl;
                //لو عدل كل حاجه و  الصوره
                if (movieFile != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(movieFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\movies", fileName);
                    //copy img in the wwwroot
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        movieFile.CopyTo(stream);
                    }

                    // wwwroot كده بيعدل الصوره عاوزين بقه نمسح لصوره القديمه من 
                    var OldPathFileInWWWRoot = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\movies", OldFileInWWWRoot);
                    if (System.IO.File.Exists(OldPathFileInWWWRoot))
                    {
                        System.IO.File.Delete(OldPathFileInWWWRoot);
                    }
                    //save img path in db
                    movie.ImgUrl = fileName;
                }
                else
                    //لو عدل كل حاجه ومعدلش الصوره هخليه يرجع الصوره القديمه الل هى ف داتا بيز اصلا 
                    //movie.ImgUrl = dbContext.Movies.AsNoTracking().FirstOrDefault(e => e.Id == movie.Id).ImgUrl;
                    movie.ImgUrl = movieRepository.GetOne(e => e.Id == movie.Id, tracked: false).ImgUrl;





                //var oldActors = dbContext.ActorMovies.Where(am => am.MovieId == movie.Id);
                //dbContext.ActorMovies.RemoveRange(oldActors);
                //dbContext.SaveChanges();  

                var oldActors = actorMovieRepository.Get(am => am.MovieId == movie.Id).ToList();
                actorMovieRepository.Delete(oldActors);
                actorMovieRepository.Commit();

                // **2️⃣ إضافة الممثلين الجدد المحددين في النموذج**
                if (moviesId != null && moviesId.Any())
                {
                    foreach (var actorId in moviesId)
                    {
                        var movieActor = new ActorMovie
                        {
                            MovieId = movie.Id,
                            ActorId = actorId
                        };
                        //dbContext.ActorMovies.Add(movieActor);
                        actorMovieRepository.Create(movieActor);
                    }
                    //dbContext.SaveChanges();
                    actorMovieRepository.Commit();
                }

                // **3️⃣ تحديث بيانات الفيلم**
                //dbContext.Movies.Update(movie);
                //dbContext.SaveChanges();

                movieRepository.Edit(movie);
                movieRepository.Commit();




                //ViewBag.actor = dbContext.Actors.ToList();
                //ViewBag.movieCategory = dbContext.Categories.ToList();
                //ViewBag.movieCinema = dbContext.Cinemas.ToList();
                ViewBag.movieCategory = categoryRepository.Get().ToList();
                ViewBag.movieCinema = cinemaRepository.Get().ToList();
                ViewBag.actor = actorRepository.Get().ToList();
                return RedirectToAction("Index");
            }
            return RedirectToAction("NotFoundPage");
        }



        public IActionResult Delete(int movieId)
        {
            //var movie = dbContext.Movies.FirstOrDefault(e => e.Id == movieId);
            var movie = movieRepository.GetOne(e => e.Id == movieId);
            if (movie != null)
            {
                //Delete old img from wwwroot
                if (movie.ImgUrl != null)
                {
                    var OldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\movies", movie.ImgUrl);
                    if (System.IO.File.Exists(OldPath))
                    {
                        System.IO.File.Delete(OldPath);
                    }
                }

                //dbContext.Movies.Remove(movie);
                //dbContext.SaveChanges();
                movieRepository.Delete(movie);
                movieRepository.Commit();
                return RedirectToAction("Index");

            }
            return RedirectToAction("NotFoundPage");
        }
    }
}
