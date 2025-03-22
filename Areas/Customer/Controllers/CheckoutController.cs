using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoviePoint.Repositories.IRepositories;
using Stripe.Checkout;

namespace MoviePoint.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : Controller
    {
        private readonly IShopRepository _shopRepository;
        public CheckoutController(IShopRepository shopRepository)
        {
            this._shopRepository = shopRepository;
        }
        public IActionResult Success(int orderId)
        {
            var order = _shopRepository.GetOne(e => e.Id == orderId);
            if (order!=null)
            {
                var service = new SessionService();
                var session = service.Get(order.SessionId);

                order.PaymentStripeId = session.PaymentIntentId; 
                order.Status = true;
                order.PaymentStatus = true;
                
                _shopRepository.Commit();
            }
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}
