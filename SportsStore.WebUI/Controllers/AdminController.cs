using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
	[Authorize]
    public class AdminController : Controller
    {
		private readonly IProductRepository _repository;

		public AdminController(IProductRepository repository)
		{
			_repository = repository;
		}

		public ViewResult Index()
        {
            return View(_repository.Products);
        }

		public ViewResult Edit(int productID)
		{
			var product = _repository.Products.FirstOrDefault(p => p.ProductID == productID);
			return View(product);
		}

		[HttpPost]
		public ActionResult Edit(Product product, HttpPostedFileBase image = null)
		{
			if (ModelState.IsValid)
			{
				if (image != null)
				{
					product.ImageMimeType = image.ContentType;
					product.ImageData = new byte[image.ContentLength];
					image.InputStream.Read(product.ImageData, 0, image.ContentLength);
				}
				_repository.SaveProduct(product);
				TempData["message"] = string.Format("{0} has ben saved", product.Name);
				return RedirectToAction("Index");
			}
			else
			{
				return View(product);
			}
		}

		public ActionResult Create()
		{
			return View("Edit", new Product());
		}

		public ActionResult Delete(int productID)
		{
			Product deletedProduct = _repository.DeleteProduct(productID);

			if (deletedProduct != null)
			{
				TempData["message"] = string.Format("{0} was deleted", deletedProduct.Name);
			}

			return RedirectToAction("Index");
		}
    }
}