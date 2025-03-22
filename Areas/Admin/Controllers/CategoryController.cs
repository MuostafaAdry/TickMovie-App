using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.DataAccess;
using MoviePoint.Models;
using MoviePoint.Repositories;
using MoviePoint.Repositories.IRepositories;
using System.Reflection.Metadata.Ecma335;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MoviePoint.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepositories categoryRepository ;
        public CategoryController(ICategoryRepositories categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public IActionResult Index(string query,  int page =1)
        {
             var Categories = categoryRepository.Get();
            //filter
            if (query != null)
            {
                Categories = Categories.Where(e => e.Name.Contains(query)
                );
            }
            //pagination
            var paginationPages = (int)Math.Ceiling((decimal)Categories.Count() / 7);
            if (page > paginationPages) page = paginationPages;
            Categories = Categories.Skip((page - 1) * 7).Take(7);
            ViewBag.paginationPages = paginationPages;
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
