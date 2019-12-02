using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
	[TestClass]
	public class CartTests
	{
		private Product P1;
		private Product P2;
		private Product P3;

		private Product CreateProduct(int id, [CallerMemberName] string name = null)
			=> new Product { ProductID = id, Name = name, Price = 10M };

		public CartTests()
		{
			P1 = CreateProduct(1);
			P2 = CreateProduct(2);
			P3 = CreateProduct(3);
		}

		[TestMethod]
		public void Can_Add_New_Line()
		{
			// Arrange
			Cart cart = new Cart();

			// Act
			cart.AddItem(P1, 1);
			cart.AddItem(P2, 2);
			CartLine[] result = cart.Lines.ToArray();

			// Assert
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(P1, result[0].Product);
			Assert.AreEqual(P2, result[1].Product);
		}

		[TestMethod]
		public void Can_Add_Quantity_For_Existing_Lines()
		{
			// Arrange
			Cart cart = new Cart();

			// Act
			cart.AddItem(P1, 1);
			cart.AddItem(P2, 1);
			cart.AddItem(P1, 10);

			CartLine[] result = cart.Lines.OrderBy(l => l.Product.ProductID).ToArray();

			// Assert
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(11, result[0].Quantity);
			Assert.AreEqual(1, result[1].Quantity);
		}

		[TestMethod]
		public void Can_Remove_Line()
		{
			// Arrange
			Cart cart = new Cart();

			cart.AddItem(P1, 1);
			cart.AddItem(P2, 4);
			cart.AddItem(P3, 3);
			cart.AddItem(P1, 2);
			cart.AddItem(P2, 1);

			// Act
			cart.RemoveLine(P2);

			// Arrange
			Assert.AreEqual(2, cart.Lines.Count());
			Assert.AreEqual(0, cart.Lines.Where(l => l.Product.ProductID == P2.ProductID).Count());
		}

		[TestMethod]
		public void Can_Calculate_Total()
		{
			// Arrange
			Cart cart = new Cart();

			cart.AddItem(P1, 3);
			cart.AddItem(P2, 1);
			cart.AddItem(P3, 1);

			// Act
			var result = cart.ComputeTotalValue();

			// Assert
			Assert.AreEqual(50M, result);
		}

		[TestMethod]
		public void Can_Clear_Cart()
		{
			// Arrange
			Cart cart= new Cart();

			cart.AddItem(P1, 3);
			cart.AddItem(P2, 1);
			cart.AddItem(P3, 1);

			// Act
			cart.Clear();

			// Assert
			Assert.AreEqual(0, cart.Lines.Count());
		}

		[TestMethod]
		public void Can_Add_To_Cart()
		{
			// Arrange
			Mock<IProductRepository> mock = new Mock<IProductRepository>();
			mock.Setup(m => m.Products).Returns(new Product[]
			{
				new Product {ProductID = 1, Name = "P1", Category = "Apples"}
			}.AsQueryable());

			Cart cart = new Cart();
			CartController controller = new CartController(mock.Object, null);

			// Act
			controller.AddToCart(cart, 1, null);

			// Assert
			Assert.AreEqual(1, cart.Lines.Count());
			Assert.AreEqual(1, cart.Lines.First().Product.ProductID);
		}

		[TestMethod]
		public void Adding_Product_T0_Cart_Goes_To_Cart_Screen()
		{
			// Arrange
			Mock<IProductRepository> mock = new Mock<IProductRepository>();
			mock.Setup(m => m.Products).Returns(new Product[]
			{
				new Product {ProductID = 1, Name = "P1", Category = "Apples"}
			}.AsQueryable());

			Cart cart = new Cart();
			CartController controller = new CartController(mock.Object, null);

			// Act
			RedirectToRouteResult result = controller.AddToCart(cart, 2, "url");

			// Assert
			Assert.AreEqual(result.RouteValues["action"], "Index");
			Assert.AreEqual(result.RouteValues["returnUrl"], "url");
		}

		[TestMethod]
		public void Can_View_Cart_Contents()
		{
			// Arrange
			Cart cart = new Cart();

			CartController controller = new CartController(null, null);

			// Act
			CartIndexViewModel result = (CartIndexViewModel)controller.Index(cart, "url").ViewData.Model;

			// Assert
			Assert.AreSame(cart, result.Cart);
			Assert.AreEqual("url", result.ReturnUrl);
		}
	}
}
