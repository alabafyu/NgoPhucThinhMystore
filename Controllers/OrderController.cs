using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NgoPhucThinhMystore.Models;
using NgoPhucThinhMystore.Models.ViewModel;


namespace _24DH113423_MyStore.Controllers
{
    public class OrderController : Controller
    {
        private MystoreEntities db = new MystoreEntities();

        // GET: Order/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Order/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            // Kiểm tra giỏ hàng trong session
            // Nếu giỏ hàng rỗng hoặc không có sản phẩm thì chuyển về trang chủ
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            // Xác thực người dùng đã đăng nhập, nếu chưa thì chuyển hướng tới trang đăng nhập
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin khách hàng từ CSDL, nếu chưa có thì chuyển hướng tới trang Đăng nhập
            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Tạo dữ liệu hiển thị cho CheckoutVM
            var model = new CheckoutVM();
            model.CartItems = cart;
            model.TotalAmount = cart.Sum(item => item.TotalPrice); // Tổng giá trị của các mặt hàng trong giỏ
            model.OrderDate = DateTime.Now; // Lấy ngày đặt hàng

            // Lấy địa chỉ giao hàng mặc định từ bảng Customer
            model.ShippingAddress = customer.CustomerAddress;
            model.CustomerID = customer.CustomerID; // Lấy mã khách hàng
            model.Username = customer.Username; // Lấy tên đăng nhập

            return View(model);
        }
        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                // Nếu giỏ hàng rỗng, điều hướng tới trang Home
                var cart = Session["Cart"] as List<CartItem>;
                if (cart == null || !cart.Any())
                {
                    return RedirectToAction("Index", "Home");
                }

                // Xác thực người dùng và khách hàng (Giống hàm GET)
                var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
                if (customer == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Nếu người dùng chọn thanh toán Paypal, chuyển hướng sang Paypal
                if (model.PaymentMethod == "PayPal")
                {
                    return RedirectToAction("PaymentWithPaypal", "PayPal", model);
                }

                // Thiết lập PaymentStatus dựa trên PaymentMethod
                string paymentStatus = "Chưa thanh toán";
                switch (model.PaymentMethod)
                {
                    case "Tiền mặt":
                        paymentStatus = "Thanh toán tiền mặt";
                        break;
                    case "PayPal":
                        paymentStatus = "Thanh toán PayPal";
                        break;
                    case "Trả trước trả sau":
                        paymentStatus = "Trả góp";
                        break;
                    default:
                        paymentStatus = "Chưa thanh toán";
                        break;
                }

                // Tạo đơn hàng và chi tiết đơn hàng liên quan
                var order = new Order();
                order.CustomerID = customer.CustomerID;
                order.OrderDate = model.OrderDate;
                order.TotalAmount = model.TotalAmount;
                order.PaymentStatus = paymentStatus;
                //order.PaymentMethod = model.PaymentMethod;
                //order.ShippingMethod = model.ShippingMethod;
                //order.ShippingAddress = model.ShippingAddress;

                // Chuyển đổi CartItems thành OrderDetails và gán vào Order
                order.OrderDetails = cart.Select(item => new OrderDetail
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    //TotalPrice = item.TotalPrice
                }).ToList();

                // Lưu đơn hàng vào CSDL
                db.Orders.Add(order);
                db.SaveChanges(); // Lưu Order và OrderDetails cùng lúc

                // Xóa giỏ hàng khi đặt hàng thành công
                Session["Cart"] = null;

                // Điều hướng tới trang xác nhận đơn hàng
                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }
            return View(model);
        }
        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders.Include("OrderDetails").SingleOrDefault(o => o.OrderID == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
    }
}