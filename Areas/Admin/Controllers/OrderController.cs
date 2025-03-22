using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.Models;
using MoviePoint.Repositories.IRepositories;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MoviePoint.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class OrderController : Controller
    {
        private readonly IShopItemRepository _shopItemRepository;
        private readonly IApplicationUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShopRepository _shopRepository;
        public OrderController(IShopItemRepository shopItemRepository,
             IApplicationUserRepository userRepository,
             UserManager<ApplicationUser> userManager,
             IShopRepository shopRepository
            )

        {
            this._shopItemRepository = shopItemRepository;
            this._userManager = userManager;
            this._userRepository = userRepository;
            this._shopRepository = shopRepository;
        }


        public IActionResult Index(string query,int page=1)
        {

            var orders = _shopItemRepository.Get(includeProps: e => e.Include(e => e.Shop).ThenInclude(e => e.ApplicationUser)
            .Include(e => e.Movie).ThenInclude(e => e.Cinema).Include(e => e.Movie).ThenInclude(e => e.Category));
            //all order
            var totalOverALL = orders.Sum(e => e.Movie.Price * e.Count);
            ViewBag.totalOverALL = totalOverALL;

            //all Success orders
            var totalSuccess = orders.Where(e=>e.Shop.PaymentStatus==true).Sum(e => e.Movie.Price * e.Count);
            ViewBag.totalSuccess = totalSuccess;

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
            var paginationPages = (int)Math.Ceiling((decimal)orders.Count() / 7);
            if (paginationPages == 0) paginationPages = 1;
            if (page < 1) page = 1;
            if (page > paginationPages) page = paginationPages;
            orders = orders.Skip((page - 1) * 7).Take(7);
            ViewBag.paginationPages = paginationPages;


            return View(orders.ToList());
        }

        public IActionResult Refound(int orderId)
        {
            var order = _shopRepository.GetOne(e => e.Id == orderId);

            if (order != null)
            {
                if (order.PaymentStatus == true && order.PaymentStripeId != null)
                {
                    var service = new SessionService();
                    var session = service.Get(order.SessionId);

                    var refundOptions = new RefundCreateOptions
                    {
                        PaymentIntent = order.PaymentStripeId,
                        Amount = (long)order.OrderTotal,
                        Reason = RefundReasons.RequestedByCustomer
                    };

                    var refundService = new RefundService();
                    var refundSession = refundService.Create(refundOptions);

                    order.PaymentStatus = false;
                    order.Status = false;
                    order.PaymentStripeId = null;
                    _shopItemRepository.Commit();
              
                }
            }


                return View();
        }

        public IActionResult Delete(int orderId)

        {
            //var order = _shopRepository.GetOne(e => e.Id == orderId);
            //if (order!=null)
            //{
            //  _shopRepository.Delete(order);
            //  _shopRepository.Commit();
            //}

            var order = _shopItemRepository.GetOne(e => e.Id == orderId);
            if (order != null)
            {
                _shopItemRepository.Delete(order);
                _shopItemRepository.Commit();
            }

            return RedirectToAction("Index");
        }
    }
}
