using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MoviePoint.DataAccess;
using MoviePoint.Models;
using MoviePoint.Repositories;
using MoviePoint.Repositories.IRepositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MoviePoint.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin,SuperAdmin")]
    public class ActorController : Controller
    {
        //ApplicationDbContext dbContext = new ApplicationDbContext();
        //ActorRepository actorRepository = new ActorRepository();
        private readonly IActorRepositories actorRepository;
        //IActorRepositories actorRepository ;
        public ActorController(IActorRepositories actorRepository)
        {
            this.actorRepository = actorRepository;
        }
      
        public IActionResult Index(string query ,int page=1)
        {
            //var actors = dbContext.Actors;
            var actors = actorRepository.Get();
            //filter
            if (query != null)
            {
                actors = actors.Where(e => e.FirstName.Contains(query)||e.LastName.Contains(query));
                 
               
            }
            //pagination
            var paginationPages = (int)Math.Ceiling((decimal)actors.Count() / 7);
            if (page > paginationPages) page = paginationPages;
            actors = actors.Skip((page - 1) * 7).Take(7);
            ViewBag.paginationPages = paginationPages;

            return View(actors.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Actor actor ,IFormFile imgActor)
        {
            if (ModelState.IsValid)
            {

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imgActor.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images\cast", fileName);
                //copy img in the wwwroot
                using (var stream = System.IO.File.Create(filePath))
                {
                    imgActor.CopyTo(stream);
                }
                //save img path in db

                actor.ProfilePicture = fileName;
                //dbContext.Actors.Add(actor);
                //dbContext.SaveChanges();     
                
                actorRepository.Create(actor);
                actorRepository.Commit();
                return RedirectToAction("Index");
            }
 
            return View(actor);
        }


        [HttpGet]
        public IActionResult Edit(int actorId)
        {
            //var actor = dbContext.Actors.FirstOrDefault(e => e.Id == actorId);
            var actor = actorRepository.GetOne(e => e.Id == actorId);
            if (actor != null)
            {
                return View(actor);
            }
            return RedirectToAction("NotFoundPage");
        }
        [HttpPost]
        public IActionResult Edit(Actor actor, IFormFile imgActor)
        {
            if (actor != null)
            {
                //var OldFileInWWWRoot = dbContext.Actors.AsNoTracking().FirstOrDefault().ProfilePicture;
                var OldFileInWWWRoot = actorRepository.GetOne(e => e.Id == actor.Id,tracked:false).ProfilePicture;
                //لو عدل كل حاجه و  الصوره
                if (imgActor != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imgActor.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images\cast", fileName);
                    //copy img in the wwwroot
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        imgActor.CopyTo(stream);
                    }

                    // wwwroot كده بيعدل الصوره عاوزين بقه نمسح لصوره القديمه من 
                    var OldPathFileInWWWRoot = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images\cast", OldFileInWWWRoot);
                    if (System.IO.File.Exists(OldPathFileInWWWRoot))
                    {
                        System.IO.File.Delete(OldPathFileInWWWRoot);
                    }
                    //save img path in db
                    actor.ProfilePicture = fileName;
                }
                else
                    //لو عدل كل حاجه ومعدلش الصوره هخليه يرجع الصوره القديمه الل هى ف داتا بيز اصلا 
                    //actor.ProfilePicture = dbContext.Actors.AsNoTracking().FirstOrDefault(e => e.Id == actor.Id).ProfilePicture;
                    actor.ProfilePicture = actorRepository.GetOne(e => e.Id == actor.Id,tracked:false).ProfilePicture;

                //dbContext.Actors.Update(actor);
                //dbContext.SaveChanges();
                
                actorRepository.Edit(actor);
                actorRepository.Commit();
                return RedirectToAction("Index");
            }
            return RedirectToAction("NotFoundPage");
        }



        public IActionResult Delete(int actorId)
        {
            //var actor = dbContext.Actors.FirstOrDefault(e => e.Id == actorId);
            var actor = actorRepository.GetOne(e => e.Id == actorId);

            if (actor != null)
            {
                //Delete old img from wwwroot
                if (actor.ProfilePicture != null)
                {
                    var OldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images\cast", actor.ProfilePicture);
                    if (System.IO.File.Exists(OldPath))
                    {
                        System.IO.File.Delete(OldPath);
                    }
                }

                //dbContext.Actors.Remove(actor);
                //dbContext.SaveChanges(); 
                
                actorRepository.Delete(actor);
                actorRepository.Commit();
                return RedirectToAction("Index");

            }
            return RedirectToAction("NotFoundPage");
        }
    }
}
