using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Client;
using Console = Colorful.Console;
using System.Drawing;
using MyCrm;
using Microsoft.Xrm.Sdk.Query;
using static MyCrm.QueryFactory;
using CrmToJira;
using JiraSync.Objects;
using Newtonsoft.Json;
using CrmToJira.Objects;
using MyCrm.Containers;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Exchange.WebServices.Autodiscover;
using System.Net.Mail;
using System.Net;
using EarlyBoundEntities;
using Microsoft.Xrm.Sdk;

namespace JiraSync.Logging {
	public static class LoggingAndErrorHandling {

		public const string DEFAULT_USERID = "D90AAD33-E919-E711-80D2-005056A36B9B";
		public const string GMAIL_PASSWORD = "cmjvxengerlbzais";
		public const string GMAIL_SENDER = "weber.mathew@gmail.com";
		public const string GMAIL_SENDER_NAME = "Matt Weber";
		public const string GMAIL_SMTP = "smtp.gmail.com";

		public static void AttachNote(MyCrm.Containers.Note note, string jirakey, OrganizationServiceProxy Proxy) {
			try {
				Entity Annotation = new Entity("annotation");
				Annotation.Attributes["objectid"] = new EntityReference("msus_jiraissue", new Guid(note.objectid));
				Annotation.Attributes["subject"] = jirakey;
				Annotation.Attributes["notetext"] = note.notetext;
				Proxy.Create(Annotation);
			} catch (Exception e) {
				LogSync(jirakey, "Error adding note while syncing: " + jirakey + "\n\n" + e.Message, Proxy);
			}
		}

		public static void SendMail(string systemuserid, string subject, string body, OrganizationServiceProxy Proxy) {
			try {

				string recipient = GetUserEmail(new Guid(systemuserid), Proxy);

				var fromAddress = new MailAddress(GMAIL_SENDER, GMAIL_SENDER_NAME);
				var toAddress = new MailAddress(recipient);
				const string fromPassword = GMAIL_PASSWORD;

				var smtp = new SmtpClient
				{
					Host = GMAIL_SMTP,
					Port = 587,
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
				};
				using (var message = new MailMessage(fromAddress, toAddress)
				{
					Subject = subject,
					Body = body
				}) {
					smtp.Send(message);
				}
			} catch (Exception e) {

			}
		}

		public static void LogSync(string name, string msg, OrganizationServiceProxy Proxy) {
			EntityContainer container = new EntityContainer();
			container.entityFields.Add(new EntityField("msus_name", name));
			container.entityFields.Add(new EntityField("msus_message", msg));
			MyCrm.Responses.Response response = new MyCrm.Responses.Response();
			response = MyCrm.Content.CreateEntity("msus_jirasynclog", DEFAULT_USERID, container, null, Proxy);
		}

		static string GetUserEmail(Guid userid, OrganizationServiceProxy Proxy) {
			QueryFactory factory = new MyCrm.QueryFactory("systemuser");
			CrmToJira.JiraApi api = new CrmToJira.JiraApi(new Guid(DEFAULT_USERID));
			ColumnSet columns = api.GetCrmJiraColumns();
			factory.addColumn("internalemailaddress");
			QueryFactory.Filter filter = new QueryFactory.Filter();
			filter.addCondition(new Filter.FilterCondition("systemuserid", Filter.Operator.EQUALS, userid.ToString()));
			factory.setFilter(filter);
			string query = factory.construct();
			string result = MyCrm.Content.Retrieve(query, Proxy);
			Users users = Newtonsoft.Json.JsonConvert.DeserializeObject<Users>(result);
			return users.value[0].internalemailaddress;
		}

		public class User {
			public string etag { get; set; }
			public string internalemailaddress { get; set; }
			public string systemuserid { get; set; }
			public string ownerid { get; set; }
		}

		public class Users {
			public string context { get; set; }
			public List<User> value { get; set; }
		}

	}
}
