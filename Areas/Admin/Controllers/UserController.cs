using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoviePoint.Models;
using MoviePoint.Models.ViewModel;
using MoviePoint.Repositories.IRepositories;
using System.Threading.Tasks;

namespace MoviePoint.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UserController : Controller
    {
        private readonly IApplicationUserRepository _userRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(IApplicationUserRepository userRepository,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager
            )
        {
            this._userRepository = userRepository;
            this._roleManager = roleManager;
            this._userManager = userManager;
        }


        public IActionResult Index(string query, int page = 1)
        {
            IEnumerable<ApplicationUser> Users = _userRepository.Get();
            //filter
            if (query != null)
            {
                Users = Users.Where(e => e.UserName.Contains(query)
                || e.Email.Contains(query) || e.Address.Contains(query));
            }
            //pagination
            var paginationPages = (int)Math.Ceiling((decimal)Users.Count() / 7);
            if (page > paginationPages) page = paginationPages;
            Users = Users.Skip((page - 1) * 7).Take(7);
            ViewBag.paginationPages = paginationPages;
            return View(Users.ToList());
        }
        
        
        [HttpGet]
        public IActionResult CreateUser()
        {
            var roles = _roleManager.Roles.ToList();
            ViewBag.roles = roles;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreateUser(RegisterVM registerVM, string Roletype)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser()
                {
                    UserName = registerVM.UserName,
                    Email = registerVM.Email,
                    Address = registerVM.Address,
                    PhoneNumber = registerVM.PhoneNumber
                };
                var result = await _userManager.CreateAsync(applicationUser, registerVM.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(applicationUser, new List<string> { Roletype });
                }
                else
                {
                    ModelState.AddModelError("Password", "Not Match the constrainse");
                }


            }

            return RedirectToAction("Index");
        }




    }
}
