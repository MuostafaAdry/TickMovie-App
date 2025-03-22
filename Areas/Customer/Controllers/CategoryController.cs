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
        public IActionResult Index(int page=1)
        {
             //var Categories = dbContext.Categories;
            var Categories = categoryRepository.Get();
            //pagination
            var paginationPages = (int)Math.Ceiling((decimal)Categories.Count() / 7);
            if (page > paginationPages) page = paginationPages;
            Categories = Categories.Skip((page - 1) * 7).Take(7);
            ViewBag.paginationPages = paginationPages;
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
