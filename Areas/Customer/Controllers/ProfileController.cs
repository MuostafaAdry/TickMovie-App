using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoviePoint.Models;
using MoviePoint.Models.ViewModel;
using MoviePoint.Repositories.IRepositories;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoviePoint.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProfileController : Controller
    {
        private readonly IApplicationUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public ProfileController(IApplicationUserRepository userRepository,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            this._userRepository = userRepository;
            this._userManager = userManager;
            this._signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Edit()
        {
            var userId = _userManager.GetUserId(User);
            var user = _userRepository.GetOne(e => e.Id == userId);

            if (user != null)
            {
                var userData = new RegisterVM
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    ImgProfile = user.ImgProfile
                };
                return View(userData);
            }
            return View(new RegisterVM());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegisterVM registerVM, IFormFile? Img, string OldPassword)
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
                var userId = _userManager.GetUserId(User);
                var user = _userRepository.GetOne(e => e.Id == userId);

                user.UserName = registerVM.UserName;
                user.Email = registerVM.Email;
                user.Address = registerVM.Address;
                user.PhoneNumber = registerVM.PhoneNumber;
                if (Img != null)
                {
                    user.ImgProfile = registerVM.ImgProfile;
                }

                //update password with token

                //var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var changePassword = await _userManager.ResetPasswordAsync(user, token, registerVM.Password);

                if (OldPassword != null)
                {
                    var changePassword = await _userManager.ChangePasswordAsync(user, OldPassword, registerVM.Password);
                    //print errors to user
                    if (!changePassword.Succeeded)
                    {
                        foreach (var item in changePassword.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                        return View(registerVM);
                    }
                }
                 


           
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    //refresh cookies to update date after editing
                    await _signInManager.RefreshSignInAsync(user);
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(registerVM);
        }


        //[HttpGet]
        //public IActionResult Edit()
        //{
        //    var userId = _userManager.GetUserId(User);
        //    var user = _userRepository.GetOne(e => e.Id == userId);
        //    if (user!=null)
        //    {
        //        var UserData = new RegisterVM
        //        {
        //            UserName=user.UserName,
        //            Email=user.Email,
        //            Address=user.Address,
        //            PhoneNumber=user.PhoneNumber,
        //            ImgProfile=user.ImgProfile
        //        };
        //        return View(UserData);
        //    }
        //    return View(new RegisterVM());
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(RegisterVM registerVM, IFormFile? Img)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string fileName = null;

        //        if (Img != null)
        //        {
        //            fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);

        //            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images", fileName);
        //            //copy img in the wwwroot
        //            using (var stream = System.IO.File.Create(filePath))
        //            {
        //                Img.CopyTo(stream);
        //            }
        //            //save img path in db
        //            registerVM.ImgProfile = fileName;
        //        }

        //        var userId = _userManager.GetUserId(User);
        //        var user = _userRepository.GetOne(e => e.Id == userId);

        //        user.UserName = registerVM.UserName;
        //        user.Email = registerVM.Email;
        //        user.Address = registerVM.Address;
        //        user.PhoneNumber = registerVM.PhoneNumber;  
        //        // تحديث الصورة فقط إذا قام المستخدم برفع صورة جديدة
        //        if (Img != null)
        //        {
        //            user.ImgProfile = fileName;
        //        }
        //        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //        var passwordChangeResult = await _userManager.ResetPasswordAsync(user, token, registerVM.Password);
        //        if (!passwordChangeResult.Succeeded)
        //        {
        //            foreach (var error in passwordChangeResult.Errors)
        //            {
        //                ModelState.AddModelError("", error.Description);
        //            }
        //            return View(registerVM);    
        //        }

        //        var result = await _userManager.UpdateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            await _signInManager.RefreshSignInAsync(user);
        //            return RedirectToAction("Index", "Home", new {area="Customer"});
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("Password", "passwords not matched");
        //        }


        //    }
        //    return View();
        //}

    }
}
