using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Concrete
{
	public class EmailSettings
	{
		public string MailToAddress = "orders@example.com";
		public string MailFromAdress = "sportsstore@example.com";
		public bool UseSsl = true;
		public string Username = "MySmtpUsername";
		public string Password = "MySmtpPassword";
		public string ServerName = "smpt.example.com";
		public int ServerPort = 587;
		public bool WtireAsFile = false;
		public string FileLocation = @"C:\Work\PRO ASP.NET MVC\SportsStore\git\SportsStoreSite\sports_store_emails";
	}

	public class EmailOrderProcessor : IOrderProcessor
	{
		private EmailSettings emailSettings;

		public EmailOrderProcessor(EmailSettings settings)
		{
			emailSettings = settings;
		}

		public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
		{
			using (var smtpClient = new SmtpClient())
			{
				smtpClient.EnableSsl = emailSettings.UseSsl;
				smtpClient.Host = emailSettings.ServerName;
				smtpClient.Port = emailSettings.ServerPort;
				smtpClient.UseDefaultCredentials = false;
				smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

				if (emailSettings.WtireAsFile)
				{
					smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
					smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
					smtpClient.EnableSsl = false;
				}

				StringBuilder body = new StringBuilder()
					.AppendLine("A new order has been submited")
					.AppendLine("---")
					.AppendLine("Items:");

				foreach (var line in cart.Lines)
				{
					var subtotal = line.Product.Price * line.Quantity;
					body.AppendFormat("{0} x {1} (subtota: {2:c}", line.Quantity,
						line.Product.Name,
						subtotal);
				}

				body.AppendFormat("Total order value: {0}", cart.ComputeTotalValue())
					.AppendLine("---")
					.AppendLine("Ship to:")
					.AppendLine(shippingDetails.Name)
					.AppendLine(shippingDetails.Line1)
					.AppendLine(shippingDetails.Line2 ?? "")
					.AppendLine(shippingDetails.Line3 ?? "")
					.AppendLine(shippingDetails.City)
					.AppendLine(shippingDetails.State)
					.AppendLine(shippingDetails.Country)
					.AppendLine(shippingDetails.Zip ?? "")
					.AppendLine("---")
					.AppendFormat("Gift Wrap: {}",
						shippingDetails.GiftWrap ? "Yes" : "No");

				MailMessage message = new MailMessage(emailSettings.MailFromAdress, emailSettings.MailToAddress, "New order submitted!", body.ToString());

				if (emailSettings.WtireAsFile)
				{
					message.BodyEncoding = Encoding.ASCII;
				}

				smtpClient.Send(message);
			}
		}
	}
}
