using E_commerce2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoviePoint.Models;
using MoviePoint.Repositories;
using MoviePoint.Repositories.IRepositories;
using Stripe.Checkout;

namespace MoviePoint.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
       
        private readonly IShopRepository _shopRepository;
        private readonly IShopItemRepository _shopItemRepository;
        public CartController(ICartRepository cartRepository,
               UserManager<ApplicationUser> userManager,
               IShopRepository shopRepository,
               IShopItemRepository shopItemRepository
               )
        {
            this._cartRepository = cartRepository;
            this._userManager = userManager;
            this._shopRepository = shopRepository;
            this._shopItemRepository = shopItemRepository;
        }
        public IActionResult Index(int page)
        {
            var cart = _cartRepository.Get(e => e.ApplicationUserId == _userManager.GetUserId(User), includes: [e => e.Movie, e => e.ApplicationUser]);
            //total
            var total = cart.Sum(e => e.Movie.Price * e.Count);
            ViewBag.total = total;
            //cartCount in vav
            var cartCount =cart.Count();
            ViewBag.cartCount = cartCount;
            //pagination
            var paginationPages = (int)Math.Ceiling((decimal)cart.Count() / 5);
            if (paginationPages == 0) paginationPages = 1;
            if (page < 1) page = 1;
            if (page > paginationPages) page = paginationPages;
            cart = cart.Skip((page - 1) * 5).Take(5);
            ViewBag.paginationPages = paginationPages;
            return View(cart);
        }

        public IActionResult AddToCart(int productId, int count)
        {

            var Cart = new Cart()
            {
                MovieId = productId,
                Count = count,
                ApplicationUserId = _userManager.GetUserId(User)
            };
          
                

            //علشان نشوف لو النتج ده اصلا موجود يعدل الكميه فقط غير كده هيضيفه 
            //idعلشان لو انا بضيف منتج مرتين هيحصل مشكله هيبقوا بنفس 
            var productInDB = _cartRepository.GetOne(e => e.MovieId == productId && e.ApplicationUserId == _userManager.GetUserId(User));
            if (productInDB != null)
                productInDB.Count += count;
            else
                _cartRepository.Create(Cart);

            _cartRepository.Commit();
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }


        public IActionResult Increment(int productid, int page = 1)
        {
            var cartItem = _cartRepository.GetOne(e => e.MovieId == productid && e.ApplicationUserId == _userManager.GetUserId(User));
            if (cartItem != null)
            {
                cartItem.Count++;
                _cartRepository.Commit();
            }
            return RedirectToAction("Index", new { page = page });
        }

        public IActionResult Decrement(int productid, int page = 1)
        {
            var cartItem = _cartRepository.GetOne(e => e.MovieId == productid && e.ApplicationUserId == _userManager.GetUserId(User));
            if (cartItem != null && cartItem.Count > 1)
            {
                cartItem.Count--;
                _cartRepository.Commit();
            }
            return RedirectToAction("Index", new { page = page });
        }

        public IActionResult Delete(int productid)
        {
            var cartItem = _cartRepository.GetOne(e => e.MovieId == productid && e.ApplicationUserId == _userManager.GetUserId(User));
            if (cartItem != null)
            {
                _cartRepository.Delete(cartItem);
                _cartRepository.Commit();
            }
            return RedirectToAction("Index");
        }

        //handle action to pay with stripe
        public IActionResult Pay()
        {

            var cart = _cartRepository.Get(e => e.ApplicationUserId == _userManager.GetUserId(User), includes: [e => e.Movie, e => e.ApplicationUser]);
            var order = new Shop();
            order.ApplicationUserId = _userManager.GetUserId(User);
            order.OrderDate = DateTime.Now;
            order.OrderTotal = cart.Sum(e => e.Movie.Price * e.Count);
            _shopRepository.Create(order);
            _shopRepository.Commit();
         

             
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Success?orderId={order.Id}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Cancel",
            };
             
            foreach (var item in cart)
            {
                options.LineItems.Add(

                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Movie.Name,
                            Description = item.Movie.Description,
                            Images = item.Movie.ImgUrl != null ? new List<string> { item.Movie.ImgUrl } : new List<string>(),
                        },
                        UnitAmount = (long)item.Movie.Price*100,
                    },
                    Quantity = item.Count,
                }
            );
            }

            var service = new SessionService();
            var session = service.Create(options);
            order.SessionId = session.Id;
            _shopRepository.Commit();


            List<ShopItem> orderItems = [];
            foreach (var item in cart)
            {
                var orderItem = new ShopItem()
                {
                    ShopId = order.Id,
                    Count = item.Count,
                    Price = item.Movie.Price,
                    MovieId = item.MovieId
                };
                orderItems.Add(orderItem);

            }
            _shopItemRepository.CreateRange(orderItems);
            _shopItemRepository.Commit();
            return Redirect(session.Url);
        }

    }
}
