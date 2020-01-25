﻿using Moq;
using Ninject;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Concrete;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Infrastructure.Concrete;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Infrastructure
{
	public class NinjectDependencyResolver : IDependencyResolver
	{
		private readonly IKernel _kernel;

		public NinjectDependencyResolver(IKernel kernel)
		{
			_kernel = kernel;
			AddBindings();
		}

		public object GetService(Type serviceType)
		{
			return _kernel.TryGet(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return _kernel.GetAll(serviceType);
		}

		private void AddBindings()
		{
			_kernel.Bind<IProductRepository>().To<EFProductRepository>();
			EmailSettings emailSettings = new EmailSettings
			{
				WtireAsFile = bool.Parse(ConfigurationManager.AppSettings["Email.WriteAsFile"] ?? "false")
			};

			_kernel.Bind<IOrderProcessor>().To<EmailOrderProcessor>()
				.WithConstructorArgument("settings", emailSettings);

			_kernel.Bind<IAuthProvider>().To<FormsAuthProvider>();
		}
	}
}