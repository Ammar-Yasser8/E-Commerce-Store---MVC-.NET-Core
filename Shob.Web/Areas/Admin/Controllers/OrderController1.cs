using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mshop.Entities.Models;
using Mshop.Entities.Repositories;
using Mshop.Entities.ViewModels;
using myshop.Entities.ViewModels;
using Shop.Utilities;
using Stripe;
using System.Diagnostics;

namespace myshop.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.AdminRole)]
	public class OrderController1 : Controller
	{
		private readonly IUnitOfWork _unitofwork;

		[BindProperty]
		public OrderVM OrderVM { get; set; }

		public OrderController1(IUnitOfWork unitOfWork)
		{
			_unitofwork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		[HttpGet]
		public IActionResult GetData()
		{
			IEnumerable<OrderHeader> orderHeaders = _unitofwork.OrderHeader.GetAll(includeWord: "ApplicationUser");
			return Json(new { data = orderHeaders });
		}


        public IActionResult Details(int id)
        {
            var orderHeader = _unitofwork?.OrderHeader?.GetFirstOrDefualt(u => u.Id == id, includeWord: "ApplicationUser");
            var orderDetails = _unitofwork?.OrderDetail?.GetAll(x => x.OrderHeaderId == id, includeWord: "Product");

            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = orderHeader,
                OrderDetails = orderDetails,
            };

            return View(orderVM);
        }


        [HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetails()
		{
			var orderfromdb = _unitofwork.OrderHeader.GetFirstOrDefualt(u => u.Id == OrderVM.OrderHeader.Id);
			orderfromdb.Name = OrderVM.OrderHeader.Name;
			orderfromdb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
			orderfromdb.Address = OrderVM.OrderHeader.Address;
			orderfromdb.City = OrderVM.OrderHeader.City;

			if (OrderVM.OrderHeader.Carrier != null)
			{
				orderfromdb.Carrier = OrderVM.OrderHeader.Carrier;
			}

			if (OrderVM.OrderHeader.TrakcingNumber != null)
			{
				orderfromdb.TrakcingNumber = OrderVM.OrderHeader.TrakcingNumber;
			}

			_unitofwork.OrderHeader.Update(orderfromdb);
			_unitofwork.Complete();
			TempData["Update"] = "Item has Updated Successfully";
			return RedirectToAction("Details", "OrderController1", new { id = orderfromdb.Id });
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartProccess()
		{
			_unitofwork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.Proccessing, null);
			_unitofwork.Complete();
			
			TempData["Update"] = "Order Status has Updated Successfully";
			return RedirectToAction("Details", "OrderController1", new { id = OrderVM.OrderHeader.Id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartShip()
		{
			var orderfromdb = _unitofwork.OrderHeader.GetFirstOrDefualt(u => u.Id == OrderVM.OrderHeader.Id);
			orderfromdb.TrakcingNumber = OrderVM.OrderHeader.TrakcingNumber;
			orderfromdb.Carrier = OrderVM.OrderHeader.Carrier;
			orderfromdb.OrderStatus = SD.Shipped;
			orderfromdb.ShippingDate = DateTime.Now;

			_unitofwork.OrderHeader.Update(orderfromdb);
			_unitofwork.Complete();

			TempData["Update"] = "Order has Shipped Successfully";
			return RedirectToAction("Details", "OrderController1", new { id = OrderVM.OrderHeader.Id });
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder()
		{
			var orderfromdb = _unitofwork.OrderHeader.GetFirstOrDefualt(u => u.Id == OrderVM.OrderHeader.Id);
			if (orderfromdb.PaymentStatus == SD.Approve)
			{
				var option = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderfromdb.PaymentIntentId
				};

				var service = new RefundService();
				Refund refund = service.Create(option);

				_unitofwork.OrderHeader.UpdateStatus(orderfromdb.Id, SD.Cancelled, SD.Refund);
			}
			else
			{
				_unitofwork.OrderHeader.UpdateStatus(orderfromdb.Id, SD.Cancelled, SD.Cancelled);
			}
			_unitofwork.Complete();

			TempData["Delete"] = "Order has Cancelled Successfully";
			return RedirectToAction("Details", "OrderController1", new { id = OrderVM.OrderHeader.Id });
		}
	}
}
