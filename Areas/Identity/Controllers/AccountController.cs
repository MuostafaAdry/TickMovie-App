using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoviePoint.Models;
using MoviePoint.Models.ViewModel;
using System.Threading.Tasks;

namespace MoviePoint.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,RoleManager<IdentityRole> roleManager
            )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (_roleManager.Roles.IsNullOrEmpty())
            {
               await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
               await _roleManager.CreateAsync(new IdentityRole("Admin"));
               await _roleManager.CreateAsync(new IdentityRole("Company"));
               await _roleManager.CreateAsync(new IdentityRole("Customer"));
          
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM, IFormFile? Img)
        {
            if (ModelState.IsValid)
            {
                string fileName = null;

                if (Img != null)
                {               
                      fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images", fileName);
                    //copy img in the wwwroot
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        Img.CopyTo(stream);
                    }
                    //save img path in db
                    registerVM.ImgProfile = fileName;
                }

                     ApplicationUser applicationUser = new()
                     {
                        UserName = registerVM.UserName,
                        Email = registerVM.Email,
                        Address = registerVM.Address,
                        //PasswordHash=registerVM.Password,
                        PhoneNumber = registerVM.PhoneNumber,
                        ImgProfile = fileName
                     };
                    var result = await _userManager.CreateAsync(applicationUser, registerVM.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(applicationUser, "Customer");
                        return RedirectToAction("Login", "Account", new { area = "Identity" });
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Not Match the constrainse");
                    }

                 
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginVM());
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
               var userApp= await _userManager.FindByEmailAsync(loginVM.Email);
                if (userApp!=null)
                {
                    var result = await _userManager.CheckPasswordAsync(userApp, loginVM.Password);
                    if (result)
                    {

                        await _signInManager.SignInAsync(userApp, loginVM.RememberMe);
                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "error in the Password");

                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "this Email Not found");
                }
            }
            return View(loginVM);
        }
        public async Task<IActionResult> Logout()
        {
           await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area = "Identity" });

        }
    }
}
