using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mshop.DataAccess;

using Mshop.Entities.Model;
using Mshop.Entities.Models;
using Mshop.Entities.Repositories;
using Shop.Utilities;

namespace Shob.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class CategoryController1 : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CategoryController1(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var categories = _unitOfWork.Category.GetAll();
            return View(categories);
        }
        //-------------------------------------Create Action----------------------------------------------------------

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Complete();
                TempData["Create"] = "Category is Created successfully";
                return RedirectToAction("Index");
            }
            return View(category);

        }
        //-------------------------------------Edit Action----------------------------------------------------------
        [HttpGet]
        public IActionResult Edit(int? id)
        {

            var category = _unitOfWork.Category.GetFirstOrDefualt(x => x.Id == id);
            if (id == null | id == 0)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Complete();
                TempData["Update"] = "Category Updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //-------------------------------------Delete Action----------------------------------------------------------
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = _unitOfWork.Category.GetFirstOrDefualt(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = _unitOfWork.Category.GetFirstOrDefualt(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(category);
            _unitOfWork.Complete();
            TempData["Delete"] = "Category is deleted successfully";
            return RedirectToAction(nameof(Index));
        }

    }
}
