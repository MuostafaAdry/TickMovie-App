using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePoint.DataAccess;
using MoviePoint.Models;
using MoviePoint.Repositories;
using MoviePoint.Repositories.IRepositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MoviePoint.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CinemaController : Controller
    {
        //ApplicationDbContext dbContext = new ApplicationDbContext();
        //CinemaRepository cinemaRepository = new CinemaRepository();
        // CinemaRepository cinemaRepository ;
        private readonly ICinemaRepositories cinemaRepository ;
        public CinemaController(ICinemaRepositories cinemaRepository)
        {
            this.cinemaRepository = cinemaRepository;
        }
        public IActionResult Index(string query,int page =1)
        {
            //var cinemas = dbContext.Cinemas;
            var cinemas = cinemaRepository.Get();
            //filter
            if (query != null)
            {
                cinemas = cinemas.Where(e => e.Name.Contains(query));
            }
            //pagination
            var paginationPages = (int)Math.Ceiling((decimal)cinemas.Count() / 7);
            if (page > paginationPages) page = paginationPages;
            cinemas = cinemas.Skip((page - 1) * 7).Take(7);
            ViewBag.paginationPages = paginationPages;
            return View(cinemas.ToList());
        }

        //handel Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Cinema());
        }
        [HttpPost]
        public IActionResult Create(Cinema cinema )
        {
            if (ModelState.IsValid)
            {
                if (cinema!=null)
                {
                    //dbContext.Cinemas.Add(cinema);
                    //dbContext.SaveChanges();
                    
                    cinemaRepository.Create(cinema);
                    cinemaRepository.Commit();
                    return RedirectToAction("Index");
                }
            }
            return View(cinema);

        }

        //handel edit

        [HttpGet]
        public IActionResult Edit(int cinemaId)
        {
            //var cinema = dbContext.Cinemas.FirstOrDefault(e => e.Id == cinemaId);
            var cinema = cinemaRepository.GetOne(e => e.Id == cinemaId);
            if (cinema != null)
            {
                return View(cinema);
            }
            return View();
        }
        [HttpPost]
        public IActionResult Edit(Cinema cinema)
        {
            if (ModelState.IsValid)
            {
                if (cinema!=null)
                {
                    //dbContext.Cinemas.Update(cinema);
                    //dbContext.SaveChanges();
                    
                    cinemaRepository.Edit(cinema);
                    cinemaRepository.Commit();
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        public IActionResult Delete(int cinemaId)
        {
            //var cinema = dbContext.Cinemas.FirstOrDefault(e => e.Id == cinemaId);
            var cinema = cinemaRepository.GetOne(e => e.Id == cinemaId);
            if (cinema!=null)
            {
                //dbContext.Cinemas.Remove(cinema);
                //dbContext.SaveChanges(); 
                
                cinemaRepository.Delete(cinema);
                cinemaRepository.Commit();
            }
            return RedirectToAction("Index");
        }
    }
}
