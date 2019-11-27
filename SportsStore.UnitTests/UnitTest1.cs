using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Models;


namespace SportsStore.UnitTests
{
	[TestClass]
	public class UnitTest1
	{
		private IProductRepository _repository;
		private IProductRepository CreateMockRepository()
		{
			if (_repository != null) return _repository;

			Mock<IProductRepository> mock = new Mock<IProductRepository>();
			mock.Setup(m => m.Products).Returns(new Product[] {
				new Product { ProductID = 1, Name = "P1", Category = "Oranges" },
				new Product { ProductID = 2, Name = "P2", Category = "Apples" },
				new Product { ProductID = 3, Name = "P3", Category = "Oranges" },
				new Product { ProductID = 4, Name = "P4", Category = "Apples" },
				new Product { ProductID = 5, Name = "P5", Category = "Plums" },
			});

			_repository = mock.Object;
			return _repository;
		}

		[TestMethod]
		public void Can_Paginate()
		{
			// Arrange
			ProductController controller = new ProductController(CreateMockRepository());
			controller.PageSize = 3;

			// Act
			ProductListViewModel result = (ProductListViewModel)controller.List(null, 2).Model;

			// Assert
			var prodArray = result.Products.ToArray();
			Assert.IsTrue(prodArray.Length == 2);
			Assert.AreEqual(prodArray[0].Name, "P4");
			Assert.AreEqual(prodArray[1].Name, "P5");
		}

		//[TestMethod]
		//public void Can_Generate_Page_Links()
		//{
		//	// Arragne
		//	HtmlHelper helper = null;

		//	PagingInfo pagingInfo = new PagingInfo()
		//	{
		//		CurrentPage = 2,
		//		TotalItems = 28,
		//		ItemsPerPage = 10
		//	};

		//	Func<int, string> pageUrlDelegate = i => "Page" + i;

		//	// Act
		//	MvcHtmlString result = helper.PageLinks(pagingInfo, pageUrlDelegate);

		//	// Assert
		//	Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
		//		+ @"<a class=""btn btn-default"" href=""Page2"">2</a>"
		//		+ @"<a class=""btn btn-default"" href=""Page3"">3</a>",
		//	result.ToString().Replace("\\", string.Empty));
		//}

		[TestMethod]
		public void Can_Send_Pagination_View_Model()
		{
			// Arrange
			ProductController controller = new ProductController(CreateMockRepository());
			controller.PageSize = 3;

			// Act
			ProductListViewModel result = (ProductListViewModel)controller.List(null, 2).Model;

			// Assert
			PagingInfo pagingInfo = result.PagingInfo;
			Assert.AreEqual(pagingInfo.CurrentPage, 2);
			Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
			Assert.AreEqual(pagingInfo.TotalItems, 5);
			Assert.AreEqual(pagingInfo.TotalPages, 2);
		}

		[TestMethod]
		public void Can_Filter_Products()
		{
			// Arrange
			ProductController controller = new ProductController(CreateMockRepository());
			controller.PageSize = 3;

			// Action
			Product[] result = ((ProductListViewModel)controller.List("Apples", 1).Model).Products.ToArray();

			// Assert
			Assert.AreEqual(result.Length, 2);
			Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Apples");
			Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Apples");
		}

		[TestMethod]
		public void Can_Create_Categories()
		{
			// Arrange
			NavController controller = new NavController(CreateMockRepository());

			// Act
			string[] categories = ((IEnumerable<string>)controller.Menu().Model).ToArray();

			// Assert
			Assert.AreEqual(categories.Length, 3);
			Assert.AreEqual(categories[0], "Apples");
			Assert.AreEqual(categories[1], "Oranges");
			Assert.AreEqual(categories[2], "Plums");

		}

		[TestMethod]
		public void Indicates_Selected_Category()
		{
			// Arrange
			NavController controller = new NavController(CreateMockRepository());

			string selectedCategory = "Apples";

			// Act
			string result = controller.Menu(selectedCategory).ViewBag.SelectedCategory;

			// Assert
			Assert.AreEqual(selectedCategory, result);
		}

		[TestMethod]
		public void Generate_Category_Specific_Product_Count()
		{
			// Arrange
			ProductController controller = new ProductController(CreateMockRepository());
			controller.PageSize = 3;

			// Act
			var rez1 = ((ProductListViewModel)controller.List("Apples").Model).PagingInfo.TotalItems;
			var rez2 = ((ProductListViewModel)controller.List("Plums").Model).PagingInfo.TotalItems;
			var rezAll = ((ProductListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

			// Assert
			Assert.AreEqual(2, rez1);
			Assert.AreEqual(1, rez2);
			Assert.AreEqual(5, rezAll);

		}
	}
}
