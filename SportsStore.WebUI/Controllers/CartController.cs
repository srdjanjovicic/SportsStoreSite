using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
	public class CartController : Controller
	{
		private readonly IProductRepository _repository;
		public CartController(IProductRepository repository)
		{
			_repository = repository;
		}

		public ViewResult Index(Cart cart, string returnUrl)
		{
			return View(new CartIndexViewModel()
			{
				Cart = cart,
				ReturnUrl = returnUrl
			});
		}

		public RedirectToRouteResult AddToCart(Cart cart, int productId, string returnUrl)
		{
			Product product = _repository.Products
				.FirstOrDefault(p => p.ProductID == productId);

			if (product != null)
			{
				cart.AddItem(product, 1);
			}
			return RedirectToAction("Index", new { returnUrl });
		}
  
		public RedirectToRouteResult RemoveFromCart(Cart cart, int productId, string returnUrl)
		{
			Product product = _repository.Products
				.FirstOrDefault(p => p.ProductID == productId);

			if (product != null)
			{
				cart.RemoveLine(product);
			}
			return RedirectToAction("Index", new { returnUrl });
		}
	}
}