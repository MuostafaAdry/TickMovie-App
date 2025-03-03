using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.DataAccess;
using MoviePoint.Models;

namespace MoviePoint.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CategoryController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        public IActionResult Index()
        {
            //ViewBag.Categories = dbContext.Categories.ToList();
            var Categories = dbContext.Categories;
            return View(Categories.ToList());
        }

        public IActionResult ShowMovies(int categoryId)
        {
            //ViewBag.Categories = dbContext.Categories.ToList();
            var CategoryMovies = dbContext.Movies.Include(e=>e.Category).Include(e=>e.Cinema).Where(e=>e.CategoryId == categoryId);
            return View(CategoryMovies.ToList());
        }
 

    }
}
