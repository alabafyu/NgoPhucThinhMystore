using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NgoPhucThinhMystore.Models.ViewModel;
using NgoPhucThinhMystore.Models;


namespace NgoPhucThinhMystore.Controllers
{
    public class OrderController : Controller
    {
        private MystoreEntities db = new MystoreEntities();

        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        // GET: Order/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            // Kiểm tra giỏ hàng trong session,
            // nếu giỏ hàng rỗng hoặc không có sản phẩm thì chuyển hướng về trang chủ
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            // Xác thực người dùng đã đăng nhập chưa, nếu chưa thì chuyển hướng tới trang Đăng nhập
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin khách hàng từ CSDL, nếu chưa có thì chuyển hướng tới trang Đăng nhập
            // nếu có rồi thì lấy địa chỉ của khách hàng và gán vào ShippingAddress của CheckoutVM
            var customer = db.Customers.SingleOrDefault(c => c.Username == User.Identity.Name);
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new CheckoutVM(); // Tạo dữ liệu hiển thị cho CheckoutVM
            model.CartItems = cart; // Lấy danh sách các sản phẩm trong giỏ hàng
            model.TotalAmount = cart.Sum(item => item.TotalPrice); // Tổng giá trị của các mặt hàng trong giỏ
            model.OrderDate = DateTime.Now; // Mặc định lấy ngày thời điểm đặt hàng
            model.ShippingAddress = customer.CustomerAddress; // Lấy địa chỉ mặc định từ bảng Customer
            model.CustomerID = customer.CustomerID; // Lấy mã khách hàng từ bảng Customer
            model.Username = customer.Username; // Lấy tên đăng nhập từ bảng Customer


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
                var cart = Session["Cart"] as List<CartItem>;
                // Nếu giỏ hàng rỗng điều hướng tới trang Home
                if (cart == null || !cart.Any())
                {
                    return RedirectToAction("Index", "Home");
                }
                // Nếu người dùng chưa đăng nhập sẽ điều hướng tới trang Login
                var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                // Nếu khách hàng không khớp với tên đăng nhập, sẽ điều hướng tới trang Login
                var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
                if (customer == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                // Nếu người dùng chọn thanh toán bằng Paypal, sẽ điều hướng tới trang PaymentWithPaypal
                if (model.PaymentMethod == "Paypal")
                {
                    return RedirectToAction("PaymentWithPaypal", "Paypal", model);
                }

                // Thiết lập PaymentStatus dựa trên PaymentMethod
                string PaymentStatus = "Chưa thanh toán";
                switch (model.PaymentMethod)
                {
                    case "Tiền mặt": PaymentStatus = "Thanh toán tiền mặt"; break;
                    case "Paypal": PaymentStatus = "Thanh toán paypal"; break;
                    case "Trả sau trã trước": PaymentStatus = "Trả góp"; break;
                    default: PaymentStatus = "Chưa thanh toán"; break;
                }

                // Tạo đơn hàng và chi tiết đơn hàng liên quan
                var order = new Order();
                    CustomerID = customer.CustomerID;
                    OrderDate = model.OrderDate;
                    TotalAmount = model.TotalAmount;
                    PaymentStatus = paymentStatus;
                    PaymentMethod = model.PaymentMethod,
                    ShippingMethod = model.ShippingMethod,
                    ShippingAddress = model.ShippingAddress,
                    OrderDetails = cart.Select(item => new OrderDetail
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList();

                // Lưu đơn hàng vào CSDL
                db.Orders.Add(order);
                db.SaveChanges();

                // Xóa giỏ hàng sau khi đặt hàng thành công
                Session["Cart"] = null;
                // Điều hướng tới trang Xác nhận đơn hàng
                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }

            return View(model);
        }
        public ActionResult OrderSuccess(int id)
        {
            // Tìm kiếm đơn hàng trong cơ sở dữ liệu (db) theo OrderID.
            // .Include("OrderDetails") đảm bảo rằng thông tin chi tiết của đơn hàng (các mặt hàng) 
            // cũng được tải cùng lúc (Eager Loading) để hiển thị trên View.
            var order = db.Orders.Include("OrderDetails").SingleOrDefault(o => o.OrderID == id);

            // Kiểm tra nếu không tìm thấy đơn hàng nào có OrderID tương ứng
            if (order == null)
            {
                // Trả về lỗi 404 (Not Found)
                return HttpNotFound(); // Hoặc NotFound() trong ASP.NET Core
            }

            // Nếu tìm thấy, truyền đối tượng 'order' này tới View tương ứng (OrderSuccess.cshtml)
            return View(order);
        }
    }
}