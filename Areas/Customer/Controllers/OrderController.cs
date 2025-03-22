using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.Models;
using MoviePoint.Repositories;
using MoviePoint.Repositories.IRepositories;
using Stripe;
using Stripe.Checkout;
using System.Linq;

namespace MoviePoint.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly IShopItemRepository _shopItemRepository;
         private readonly UserManager<ApplicationUser> _userManager;
        
        public OrderController(IShopItemRepository shopItemRepository,      
             UserManager<ApplicationUser> userManager
            )

        {
            this._shopItemRepository = shopItemRepository;
            this._userManager = userManager;
 
        }


        public IActionResult Index(string query, int page = 1)
        {


            var orders = _shopItemRepository.Get(e => e.Shop.ApplicationUserId == _userManager.GetUserId(User)
            ,includeProps: e => e.Include(e => e.Shop)
            .ThenInclude(e => e.ApplicationUser)
            .Include(e => e.Movie).ThenInclude(e => e.Cinema).Include(e => e.Movie).ThenInclude(e => e.Category)) ;

            // total to all orders that the customer completed(payed)
            var total = orders.Where(e=>e.Shop.PaymentStatus==true).Sum(e => e.Price * e.Count);
            ViewBag.total = total;

            //filter
            if (query != null)
            {
                orders = orders.Where(e => e.Movie.Name.Contains(query)

                || e.Movie.Cinema.Name.Contains(query)
                || e.Shop.ApplicationUser.UserName.Contains(query)
                || e.Shop.ApplicationUser.Email.Contains(query)
                 || e.Shop.ApplicationUser.PhoneNumber.Contains(query)
                );
            }
            //pagination
            //var paginationPages = (int)Math.Ceiling((decimal)orders.Count() / 7);
            //if (page > paginationPages) page = paginationPages;
            //orders = orders.Skip((page - 1) * 7).Take(7);
            //ViewBag.paginationPages = paginationPages;

            //pagination to avoid offset problem
            var paginationPages = (int)Math.Ceiling((decimal)orders.Count() / 7);
            if (paginationPages == 0) paginationPages = 1;
            if (page < 1) page = 1;
            if (page > paginationPages) page = paginationPages;

            orders = orders.Skip((page - 1) * 7).Take(7);
            ViewBag.paginationPages = paginationPages;



            return View(orders.ToList());
        }


      
       



    }
}
