using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
	public class UrlRoutingController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Controller = "UrlRouting";
			ViewBag.Action = "Index";
			return View("ActionName");
		}

		public ActionResult CustomVariable(string id)
		{
			ViewBag.Controller = "UrlRouting";
			ViewBag.Action = "CustomVariable";
			ViewBag.CustomVariable = id ?? "<no value>";
			return View("ActionName");
		}
	}

	public class CustomerController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Controller = "Customer";
			ViewBag.Action = "Index";
			return View("ActionName");
		}
  
		public ActionResult List()
		{
			ViewBag.Controller = "Customer";
			ViewBag.Action = "List";
			return View("ActionName");
		}
	}

	public class AdminsController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Controller = "Admin";
			ViewBag.Action = "Index";
			return View("ActionName");
		}
	}
}