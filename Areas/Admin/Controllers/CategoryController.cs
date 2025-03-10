using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.DataAccess;
using MoviePoint.Models;
using MoviePoint.Repositories;
using MoviePoint.Repositories.IRepositories;
using System.Reflection.Metadata.Ecma335;

namespace MoviePoint.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        //ApplicationDbContext dbContext = new ApplicationDbContext();
        //CategoryRepository categoryRepository = new CategoryRepository();
        //CategoryRepository categoryRepository;
        private readonly ICategoryRepositories categoryRepository ;
        public CategoryController(ICategoryRepositories categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
            //var Categories = dbContext.Categories;
            var Categories = categoryRepository.Get();
            return View(Categories.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category != null)
                {
                    //dbContext.Categories.Add(category);
                    //dbContext.SaveChanges();    
                    
                    categoryRepository.Create(category);
                    categoryRepository.Commit();
                }
            }
            return RedirectToAction("Index");
        }


        //handle edit category
        [HttpGet]
        public IActionResult Edit(int categoryId)
        {
            //var category = dbContext.Categories.FirstOrDefault(e => e.Id == categoryId);
            var category = categoryRepository.GetOne(e => e.Id == categoryId);
            if (category != null)
            {
                return View(category);
            }
            return View();
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category != null)
                {
                    //dbContext.Categories.Update(category);
                    //dbContext.SaveChanges();
                    
                    categoryRepository.Edit(category);
                    categoryRepository.Commit();
                }
                return RedirectToAction("Index");
            }
            return View(category);


        }


        //hande delete category  
        public IActionResult Delete(int categoryId)
        {
            //var category = dbContext.Categories.FirstOrDefault(e => e.Id == categoryId);
            var category = categoryRepository.GetOne(e => e.Id == categoryId);
            if (category !=null)
            {
                //dbContext.Categories.Remove(category);
                //dbContext.SaveChanges();
                
                categoryRepository.Delete(category);
               categoryRepository.Commit();
            }
            return RedirectToAction("Index");
        }
    }
}
