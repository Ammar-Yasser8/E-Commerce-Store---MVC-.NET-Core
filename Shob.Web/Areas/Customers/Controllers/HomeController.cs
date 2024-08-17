using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Mshop.Entities.Models;
using Mshop.Entities.Repositories;
using Shop.Utilities;
using System.Security.Claims;
using X.PagedList.Extensions;
using X.PagedList.Mvc;

namespace Shob.Web.Areas.Customers.Controllers
{
    [Area("Customers")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int ? page)
        {
            var PageNumber = page ?? 1 ;
            int PageSize = 8;
            var product = _unitOfWork.Product.GetAll().ToPagedList(PageNumber, PageSize);
            return View(product);
        }

        public IActionResult Details(int? ProductId)
        {
            if (!ProductId.HasValue)
            {
                return NotFound();
            }

            var product = _unitOfWork.Product.GetFirstOrDefualt(x => x.Id == ProductId.Value, includeWord: "Category");
            if (product == null)
            {
                return NotFound();
            }

            ShoppingCart obj = new ShoppingCart()
            {
                Product = product,
                Count = 1
            };
            
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart Cartobj = _unitOfWork.ShoppingCart.GetFirstOrDefualt(
                u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);

            if (Cartobj == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                HttpContext.Session.SetInt32(SD.SessionKey,
                _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count()
                );
                _unitOfWork.Complete();
            }
            else
            {
                _unitOfWork.ShoppingCart.Increasecount(Cartobj, shoppingCart.Count);
                
            }
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }

    }
}
