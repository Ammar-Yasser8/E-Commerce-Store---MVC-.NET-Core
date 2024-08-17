using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mshop.DataAccess;

using Mshop.Entities.Model;
using Mshop.Entities.Models;
using Mshop.Entities.Repositories;
using Mshop.Entities.ViewModels;
using Shop.Utilities;

namespace Shob.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class ProductController1 : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;
        public ProductController1(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;   

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var categories = _unitOfWork.Product.GetAll(includeWord: "Category");
            return Json(new { data = categories });
        }
        //-------------------------------------Create Action----------------------------------------------------------

        [HttpGet]
        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM productVM, IFormFile file)
         {

            if (ModelState.IsValid)
            {
                string RootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                { 
                    string filename = Guid.NewGuid().ToString();    
                    var Upload =Path.Combine(RootPath, @"Images\Product\");
                    var ext = Path.GetExtension(file.FileName);
                    using (var filestream =new FileStream(Path.Combine(Upload,filename+ext),FileMode.Create))
                    {
                        file.CopyTo(filestream);    
                    }
                    productVM.Product.Img = @"Images\Product\" + filename + ext;


                }
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Complete();
                TempData["Create"] = "Product is Created successfully";
                return RedirectToAction("Index");
            }
            return View(productVM.Product);

        }
        //-------------------------------------Edit Action----------------------------------------------------------
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null | id == 0)
            {
                NotFound();
            }

            ProductVM productVM = new ProductVM()
            {
                Product = _unitOfWork.Product.GetFirstOrDefualt(x => x.Id == id),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                string uploadPath = Path.Combine(rootPath, "Images", "Products");

                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(file.FileName);

                    // Ensure the upload directory exists
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    if (productVM.Product.Img != null)
                    {
                        var oldImgPath = Path.Combine(rootPath, productVM.Product.Img.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }

                    string newImgPath = Path.Combine(uploadPath, filename + extension);
                    using (var fileStream = new FileStream(newImgPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.Img = Path.Combine("Images", "Products", filename + extension);
                }

                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Complete();
                TempData["Update"] = "Data has Updated Successfully";
                return RedirectToAction("Index");
            }

            return View(productVM);
        }


        //-------------------------------------Delete Action----------------------------------------------------------

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productIndb = _unitOfWork.Product.GetFirstOrDefualt(x => x.Id == id);
            if (productIndb == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            _unitOfWork.Product.Remove(productIndb);
            var oldimg = Path.Combine(_webHostEnvironment.WebRootPath, productIndb.Img.TrimStart('\\'));
            if (System.IO.File.Exists(oldimg))
            {
                System.IO.File.Delete(oldimg);
            }
            _unitOfWork.Complete();
            return Json(new { success = true, message = "file has been Deleted" });
        }

    }
}
