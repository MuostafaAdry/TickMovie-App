using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.DataAccess;
using MoviePoint.Models;
using MoviePoint.Repositories;
using MoviePoint.Repositories.IRepositories;

namespace MoviePoint.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CategoryController : Controller
    {
         //ApplicationDbContext dbContext = new ApplicationDbContext();
        private readonly ICategoryRepositories categoryRepository ;
        private readonly IMovieRepositories movieRepository ;
        public CategoryController(IMovieRepositories movieRepository, ICategoryRepositories categoryRepository)
        {
            this.movieRepository = movieRepository;
            this.categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
             //var Categories = dbContext.Categories;
            var Categories = categoryRepository.Get();
            return View(Categories.ToList());
        }
        public IActionResult ShowMovies(int categoryId)
        {
            //var CategoryMovies = dbContext.Movies
            //    .Include(e => e.Category)
            //    .Include(e => e.Cinema).
            //    Where(e=>e.CategoryId == categoryId);

            var CategoryMovies = movieRepository.Get(includes: [e => e.Category, e => e.Cinema]).Where(e => e.CategoryId == categoryId);



            return View(CategoryMovies.ToList());
        }
       
 

    }
}
