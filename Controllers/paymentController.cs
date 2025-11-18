using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NgoPhucThinhMystore.Models;
using NgoPhucThinhMystore.Models.ViewModel;

namespace NgoPhucThinhMystore.Areas.Admin.Controllers
{
    public class paymentController : Controller
    {
        // GET: Admin/payment
        private MystoreEntities db = new MystoreEntities();

        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        // GET: /Payment hoặc /Payment/Index
        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();

            // Nếu giỏ trống thì quay lại giỏ hàng
            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                return RedirectToAction("Index2", "Cart");
            }

            return View(cart);  // View: Views/Payment/Index.cshtml
        }
    }
    
}