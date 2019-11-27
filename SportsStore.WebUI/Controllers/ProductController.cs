using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
		private readonly IProductRepository _repository;
		public int PageSize = 4;

		public ProductController(IProductRepository repository)
		{
			_repository = repository;
		}

		public ViewResult List(string category, int page = 1)
		{
			var products = _repository.Products
				.Where(p => category == null || p.Category == category)
				.OrderBy(p => p.ProductID);

			ProductListViewModel model = new ProductListViewModel()
			{
				Products = products.Skip((page - 1) * PageSize).Take(PageSize),
				PagingInfo = new PagingInfo()
				{
					CurrentPage = page,
					ItemsPerPage = PageSize,
					TotalItems = products.Count()
				},
				CurrentCategory = category
			};

			return View(model);
		}
    }
}