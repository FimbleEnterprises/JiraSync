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
using static JiraSync.Logging.LoggingAndErrorHandling;
using MyCrm.Containers;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Exchange.WebServices.Autodiscover;
using System.Net.Mail;
using System.Net;
using EarlyBoundEntities;
using Microsoft.Xrm.Sdk;
using JIRA_API_Proxy.Objects;
using System.Windows.Forms;
using System.IO;
using static System.Reflection.Assembly;
using static MyCrm.QueryFactory.Filter;

namespace JiraSync {
	class Program {

		static OrganizationServiceProxy Proxy { get; set; }
		const string MATT_USERID = "3ECF2393-C71D-E711-80D2-005056A36B9B";
		const string TEST_JIRA_ISSUEID = "b20295ef-2e4c-ea11-8102-005056a36b9b";
		const string TEST_JIRA_NUM = "SR-10402";


		static void Main(string[] args) {
			// Proxy = Connector.GetProxy();
			// SendMail("3ECF2393-C71D-E711-80D2-005056A36B9B", "TEST SUBJECT", "TEST BODY", Proxy);

			if (args != null && args.Length > 0) {
				foreach (string arg in args) {
					switch (arg) {
						case "test":
							Testing();
							break;
						case "sync":
							CheckForUpdatedIssues();
							break;
						case "scrap":
							// MakeJirasForScrappedProbes(); Not working right
							break;
						default:
							break;
					}
				}
			} else {
				GetCrmJiras();
			}
		}

		static void Testing() {

			JiraCredentials credentials = new JiraCredentials();
			credentials.Username = "mweber";
			credentials.Password = "mweber";

			CrmToJira.JiraApi api = new CrmToJira.JiraApi(new Guid(DEFAULT_USERID));

			// api.ResolveIssue(TEST_JIRA_NUM, credentials, "Incomplete", false);
			api.UpdateJira(TEST_JIRA_NUM, TEST_JIRA_ISSUEID, credentials);

			/*
			Console.WriteLine("-= TEST MODE =-", Color.Firebrick);
			Console.WriteLine("Connecting to CRM...", Color.Firebrick);
			CrmToJira.JiraApi api = new CrmToJira.JiraApi(new Guid(DEFAULT_USERID));
			JiraCredentials credentials = new JiraCredentials();
			credentials.Username = "mweber";
			credentials.Password = "mweber";
			Console.WriteLine("Connected.", Color.Firebrick);*/

			// MarkScrappedAsScrapped();
			// MakeJirasForScrappedProbes();

			/*bool isCurrentlyOpen = api.IsIssueOpen("CC-8275");
			MessageBox.Show("Is open presently = " + isCurrentlyOpen.ToString());
			// bool wasResolved = api.ResolveIssue("CC-8275", credentials, "Fixed");
			bool wasResolved = api.ResolveIssue("CC-8275", credentials, "Warranty Replacement");
			bool wasClosed = api.IsIssueOpen("CC-8275");
			MessageBox.Show("Was resolved: " + wasResolved.ToString() + "\nWas closed: " + wasClosed.ToString());
			*/
		}

		static void CheckForUpdatedIssues() {
			Console.WriteLine("Connecting to CRM...", Color.Firebrick);
			CrmToJira.JiraApi api = new CrmToJira.JiraApi(new Guid(DEFAULT_USERID));
			Console.WriteLine("Connected.  Searching Jira for updated issues...", Color.Firebrick);
			JiraSearchResults jiraSearchResults = api.SearchJira();
			Console.WriteLine("Retrieved " + jiraSearchResults.total + " Jira issues.  Getting CRM issues...", Color.Firebrick);
			CrmJiraIssuesLite crmSearchResults = api.GetCrmJiraIssuesModifiedDate();
			Console.WriteLine("Retrieved " + crmSearchResults.value.Count +" CRM issues.  Comparing...", Color.Firebrick);
			List<string> UpdatedCrms = new List<string>();
			List<string> UpdatedJiras = new List<string>();

			foreach (JiraSync.Objects.Issue jiraissue in jiraSearchResults.issues) {
				// Convert to universal time to account for time zones
				DateTime jiraModifiedOn = jiraissue.fields.updated.ToUniversalTime();

				if (crmSearchResults != null && crmSearchResults.value.Count > 0) {
					foreach (CrmIssue crmissue in crmSearchResults.value) {

						MyConsole.WriteToSameLine("Comparing Jira issue " + jiraissue.key + " to CRM Jira issue " + crmissue.msus_jiranumber, Color.Green);

						// Convert to universal time to account for time zones
						DateTime crmModifiedOn = crmissue.modifiedon.ToUniversalTime();

						// If the issue numbers match
						if (crmissue.msus_jiranumber.Equals(jiraissue.key)) {

							#region IF JIRA IS NEWER (NOT IMPLEMENTED)

							// if the crm side is older pull in from jira
							/*if (crmModifiedOn < jiraModifiedOn) {

								// convert ticks to seconds
								long cTicks = crmModifiedOn.Ticks;
								long jTicks = jiraModifiedOn.Ticks;
								long diff = Math.Abs(cTicks - jTicks);
								long secs = (diff / 10000000);

								// make sure the difference is at least 30 seconds
								if (secs > 30) {
									if (!api.CrmAndJiraAreEqual(jiraissue.key, crmissue.msus_jiraissueid)) {
										Console.WriteLine("\nFound an updated Jira issue (" + jiraissue.key + "), will update the CRM issue...", Color.Aqua);
										try {
											// Try to update the crm issue from Jira
											JiraRefreshResponse refreshResponse
												= api.RefreshCrmFromJira(crmissue.msus_jiraissueid, jiraissue.key, crmissue._ownerid_value);
											bool wasSuccessful = !refreshResponse.WasFaulted;
											if (wasSuccessful) {
												Console.WriteLine("Refreshed the CRM issue");
												UpdatedCrms.Add(jiraissue.key + " was refreshed in CRM");
											} else {
												Console.WriteLine("Failed to update", Color.Red);
												Logging.LoggingAndErrorHandling.LogSync("Error updating CRM side (" + jiraissue.key + ")", "", Proxy);
												SendMail(crmissue._ownerid_value, "Error updating CRM side (" + jiraissue.key + ")", "", Proxy);
											}
										} catch (Exception e1) {
											Logging.LoggingAndErrorHandling.LogSync("Error updating CRM side (" + jiraissue.key + ")",
												e1.Message + "\n\n" + e1.StackTrace, Proxy);
											SendMail(crmissue._ownerid_value, "Error updating CRM side (" + jiraissue.key + ")"
												, e1.Message + "\n\n" + e1.StackTrace, Proxy);
										}
									} // issues are not equal
									else {  // issues ARE equal
										Console.WriteLine("\nFound equal issues with unequal modified dates.  Will update CRM date to match...", Color.DarkGoldenrod);
										MyCrm.DatabaseDirect.UpdateModifiedOn("msus_jiraissue", crmissue.msus_jiraissueid, jiraissue.fields.updated.ToUniversalTime());
										Console.WriteLine("Updated.", Color.Chartreuse);
									}
								} // more than five minutes 
							} // jira is newer 
							else if (crmModifiedOn > jiraModifiedOn) {*/
								#endregion

							if (crmModifiedOn > jiraModifiedOn) {
								// convert ticks to seconds
								long cTicks = crmModifiedOn.Ticks;
								long jTicks = jiraModifiedOn.Ticks;
								long diff = Math.Abs(cTicks - jTicks);
								long secs = (diff / 10000000);

								// make sure the difference is at least 30 seconds
								if (secs > 30) {
									if (!api.CrmAndJiraAreEqual(jiraissue.key, crmissue.msus_jiraissueid)) {
										Console.WriteLine("\nFound an updated CRM issue (" + jiraissue.key + "), will update the Jira issue...", Color.Aqua);
										try {
											// Try to update the Jira from the CRM
											JiraCredentials credentials = api.GetCredentials(new Guid(crmissue._ownerid_value));
											JiraUpdateResponse updateResponse
												= api.UpdateJira(jiraissue.key, crmissue.msus_jiraissueid, credentials);
											bool wasSuccessful = !updateResponse.WasFaulted();

											if (wasSuccessful) {
												MyCrm.DatabaseDirect.UpdateModifiedOn("msus_jiraissue", crmissue.msus_jiraissueid, jiraissue.fields.updated.ToUniversalTime());
												Console.WriteLine("Updated the Jira issue");
												UpdatedJiras.Add(jiraissue.key + " was updated in Jira.");
											} else {
												Console.WriteLine("Failed to update", Color.Red);
												Logging.LoggingAndErrorHandling.LogSync("Error updating Jira side (" + jiraissue.key + ")", "", Proxy);
												SendMail(crmissue._ownerid_value, "Error updating Jira side (" + jiraissue.key + ")", "", Proxy);
											}
										} catch (Exception e1) {
											Logging.LoggingAndErrorHandling.LogSync("Error updating Jira side (" + jiraissue.key + ")",
												e1.Message + "\n\n" + e1.StackTrace, Proxy);
											SendMail(crmissue._ownerid_value, "Error updating Jira side (" + jiraissue.key + ")"
												, e1.Message + "\n\n" + e1.StackTrace, Proxy);
										}
									} // issues are not equal
									else {  // issues ARE equal
										Console.WriteLine("\nFound equal issues with unequal modified dates.  Will update CRM date to match...", Color.DarkGoldenrod);
										MyCrm.DatabaseDirect.UpdateModifiedOn("msus_jiraissue", crmissue.msus_jiraissueid, jiraissue.fields.updated.ToUniversalTime());
										Console.WriteLine("Updated.", Color.Chartreuse);
									}
								}// newer by at least 60 seconds
							}// crm is newer
						} // jira and crm keys match
					} // end for each crm issue
				} // end has search results
			} // end for each jira issue

			if (UpdatedCrms.Count > 0 || UpdatedJiras.Count > 0) {

				StringBuilder sb = new StringBuilder();
				foreach (string line in UpdatedCrms) {
					sb.AppendLine(line + "\n");
				}
				foreach (string line in UpdatedJiras) {
					sb.AppendLine(line + "\n");
				}

				Console.WriteLine("Updated: " + UpdatedCrms.Count + " CRM issues", Color.AliceBlue);
				Console.WriteLine("Updated: " + UpdatedJiras.Count + " Jira issues", Color.AliceBlue);
				LogSync("Updated: " + UpdatedCrms.Count + " CRM issues, Updated: " + UpdatedJiras.Count + " Jira issues"
					, sb.ToString(), Proxy);
			}
		}

		/// <summary>
		/// Gets the jira part name using the traditional pn
		/// </summary>
		/// <param name="partNumber"></param>
		/// <param name="proxy"></param>
		/// <returns></returns>
		static string GetJiraPartNumber(string partNumber, OrganizationServiceProxy proxy) {
			MyCrm.QueryFactory factory = new QueryFactory("msus_jirapartnumber");
			factory.addColumn("msus_name");
			Filter filter = new Filter(FilterType.AND, new FilterCondition("msus_name", Operator.CONTAINS, partNumber));
			factory.setFilter(filter);
			string query = factory.construct();
			string json = MyCrm.Content.Retrieve(query, proxy);
			JiraParts parts = JsonConvert.DeserializeObject<JiraParts>(json);
			return parts.value[0].msus_name;
		}

		/// <summary>
		/// Gets Jira issues created in CRM that do not possess a Jira key
		/// </summary>
		static void GetCrmJiras() {
			if (Proxy == null) {
				try {
					Console.WriteLine("Connecting to server...", Color.ForestGreen);
					Proxy = Connector.GetProxy();
					Console.WriteLine("Connected", Color.ForestGreen);
				} catch (Exception e) {
					SendMail("3ECF2393-C71D-E711-80D2-005056A36B9B", "CRM Failed obtain the Proxy", 
						"ERROR:\n" + e.Message +"\n\nStack:\n" + e.StackTrace, Proxy);
					Console.WriteLine("Failed to connect", Color.Red);
				}

				QueryFactory factory = new MyCrm.QueryFactory("msus_jiraissue");
				CrmToJira.JiraApi api = new CrmToJira.JiraApi(new Guid(DEFAULT_USERID));
				ColumnSet columns = api.GetCrmJiraColumns();
				factory.addColumn("msus_jiraissueid");
				factory.addColumn("ownerid");
				factory.addColumn("msus_jiranumber");
				factory.addColumn("msus_name");
				QueryFactory.Filter filter = new QueryFactory.Filter();
				filter.addCondition(new Filter.FilterCondition("msus_jiranumber", Filter.Operator.NOT_CONTAINS_DATA));
				factory.setFilter(filter);
				string query = factory.construct();
				Console.WriteLine("Getting eligible issues using query:\n" + query + "\n", Color.ForestGreen);

				try {
					string results = MyCrm.Content.Retrieve(query, Proxy);
					JiraIssueIdContainer issueids2 = JsonConvert.DeserializeObject<JiraIssueIdContainer>(results);
					JiraIssueIdContainer issueids = new JiraIssueIdContainer();
					issueids.value = new List<JiraIssueId>();

					foreach (JiraIssueId id in issueids2.value) {
						if (! id.msus_name.StartsWith("Copy of")) {
							issueids.value.Add(id);
						} else {
							Console.WriteLine("Found a, \"Copy of\" entry, will ignore it.", Color.OrangeRed);
						}
					}

					JiraCreateResponse response;
					string jirakey = "";

					string summary = "Found " + issueids.value.Count + " issues:";
					
					foreach(JiraIssueId i in issueids.value) {
						Console.WriteLine("Found msus_jiraissue with id:", Color.AliceBlue);
						Console.WriteLine(i.msus_jiraissueid, Color.AliceBlue);
					}

					foreach (JiraIssueId issue in issueids.value) {
						try {

							// Create the Jira in Jira
							response = api.CreateJira(new Guid(issue.msus_jiraissueid));
							try {
								jirakey = response.key;
							} catch (Exception e3) { }

							if (!response.Fault.WasFaulted) {
								EntityContainer container = new EntityContainer();
								container.entityFields = new List<EntityField>();
								container.entityFields.Add(new EntityField("msus_jiranumber", response.key));
								MyCrm.Content.UpdateEntity(issue.msus_jiraissueid, "msus_jiraissue", container, issue._ownerid_value, null, Proxy);
								
								Console.WriteLine(response.key + " was created", Color.ForestGreen);
								LogSync(response.key + " was synced", "", Proxy);
							} else {
								Console.WriteLine("Error:\n" + response.Fault.FaultMessage, Color.Red);
								SendMail(issue._ownerid_value, "JiraSync failed (" + jirakey + ")", "Error:\n" 
									+ response.Fault.FaultMessage + "\n\nStack:\n" + response.Fault.FaultStackTrace, Proxy);
								LogSync(response.Fault.FaultMessage + "(" + jirakey + ")", response.Fault.FaultStackTrace, Proxy);
							}
						} catch (Exception e1) {
							SendMail(issue._ownerid_value, "CRM Failed to sync a Jira (" + jirakey + ")", e1.Message, Proxy);
							LogSync("(" + jirakey + ") " + e1.Message, e1.StackTrace, Proxy);
							Console.WriteLine(e1.Message, Color.Red);
						}
					}
				} catch (Exception e2) {
					LogSync(e2.Message, e2.StackTrace, Proxy);
					Console.WriteLine(e2.Message);
				}
			}
		}

		/*static void MakeJirasForScrappedProbes() {

			Console.WriteLine("Connecting to CRM...", Color.Orange);
			OrganizationServiceProxy proxy = MyCrm.Connector.GetProxy();
			Console.WriteLine("Connected.", Color.Green);

			CrmToJira.JiraApi api = new CrmToJira.JiraApi(new Guid(DEFAULT_USERID));

			Console.WriteLine("Querying CRM for probes to be scrapped...", Color.Orange);
			MyCrm.QueryFactory factory = new QueryFactory("msus_evaluationprobe");
			factory.addColumn("msus_evaluationprobeid");
			factory.addColumn("msus_part");
			factory.addColumn("msus_name");
			factory.addColumn("msus_serialnumber");
			factory.addColumn("msus_uses");
			factory.addColumn("ownerid");
			Filter filter = new Filter(FilterType.AND, new FilterCondition("msus_status", Operator.EQUALS, "745820005"));
			factory.setFilter(filter);
			string query = factory.construct();
			string json = MyCrm.Content.Retrieve(query, proxy);
			CrmProbes probes = JsonConvert.DeserializeObject<CrmProbes>(json);

			foreach (CrmProbe probe in probes.value) {
				Console.WriteLine("Getting Jira credentials for user: " + probe._ownerid_valueFormattedValue, Color.Orange);

				JiraCredentials credentials = api.GetCredentials(new Guid(probe._ownerid_value.ToString()));
				JiraIssue jira = new JiraIssue();
				jira.fields.SetProject("SR");
				jira.fields.SetAssignee(credentials.Username);
				jira.fields.SetSummary("Scrapping " + probe.msus_name);
				jira.fields.SetIssueType("Probe");
				jira.fields.SetPartType("Probe");
				jira.fields.SetPartNumber(GetJiraPartNumber(probe.msus_name, proxy));
				jira.fields.SetProbeType(probe.msus_name.Substring(0, 2));
				jira.fields.SetMDR("No");
				jira.fields.SetSalesRep("Medistim USA Service");
				jira.fields.SetImmediateAction("Probe was returned after evaluation.");
				jira.fields.SetDescription("Probe cannot be used clinically.\nProbe was scrapped.");
				string jiraJson = JsonConvert.SerializeObject(jira, JiraApi.serializerSettings);

				JiraCreateResponse response = api.CreateJira(jira, credentials);

				if (response.key != null && response.key.Length > 0 && ! response.Fault.WasFaulted) {
					Console.WriteLine("Created Jira (" + response.key +").  Will update CRM...", Color.Orange);

					EntityContainer container = new EntityContainer();
					container.entityFields.Add(new EntityField("msus_status", "100000000"));

					MyCrm.Responses.Response crmResponse = MyCrm.Content.UpdateEntity(probe.msus_evaluationprobeid
						, "msus_evaluationprobe", container, MATT_USERID, null, proxy);

					if (crmResponse.WasSuccessful) {
						Console.WriteLine("Updated status in CRM.  Adding note...", Color.Orange);

						MyCrm.Containers.Note note = new MyCrm.Containers.Note();
						note.notetext = "Scrapped in Jira: " + response.key;
						note.subject = "Note";
						note.objectidtypecode = "msus_evaluationprobe";
						note.objectid = probe.msus_evaluationprobeid;
						note.owneridtype = "systemuser";
						note.ownerid = MATT_USERID;

						string noteid = MyCrm.Content.AttachNote(note, proxy);

						if (noteid != null) {
							Console.WriteLine("Note added.", Color.Green);
						}
					}
				} else {
					Console.WriteLine("Failed to create Jira for s/n " + probe.msus_serialnumber, Color.Red);
				}

			}

			
		}*/

		static void MarkScrappedAsScrapped() {
			// Scrapped = 100000000

			/*Console.WriteLine("-= TEST MODE =-", Color.Firebrick);
			Console.WriteLine("Connecting to CRM...", Color.Firebrick);
			CrmToJira.JiraApi api = new CrmToJira.JiraApi(new Guid(DEFAULT_USERID));
			JiraCredentials credentials = new JiraCredentials();
			credentials.Username = "mweber";
			credentials.Password = "mweber";
			Console.WriteLine("Connected.", Color.Firebrick);*/

			OrganizationServiceProxy proxy = MyCrm.Connector.GetProxy();

			string path = "C:\\Users\\weber\\source\\repos\\CrmToolsAndUtilities\\JiraSync\\remove_json.json";
			string contents = File.ReadAllText(path);
			List <JiraSync.Program.Issue> issues = JsonConvert.DeserializeObject<List<JiraSync.Program.Issue>>(contents);
			EntityContainers containers = new EntityContainers();

			JiraCredentials credentials = new JiraCredentials();
			credentials.Username = "mweber";
			credentials.Password = "mweber";

			foreach (JiraSync.Program.Issue issue in issues) {
				Console.WriteLine("Locating CRM issue for entry: " + issue.key + " " + issue.PN + " " + issue.SN);

				MyCrm.QueryFactory factory = new QueryFactory("msus_evaluationprobe");
				factory.addColumn("msus_evaluationprobeid");
				List<FilterCondition> conditions = new List<FilterCondition>();
				conditions.Add(new FilterCondition("msus_serialnumber", Operator.EQUALS, issue.SN.ToString()));
				conditions.Add(new FilterCondition("msus_name", Operator.EQUALS, issue.PN));
				conditions.Add(new FilterCondition("msus_status", Operator.NOT_EQUALS, "100000000"));
				Filter filter = new Filter(FilterType.AND, conditions);
				factory.setFilter(filter);
				string query = factory.construct();

				string json = MyCrm.Content.Retrieve(query, proxy);
				Probe probe = JsonConvert.DeserializeObject<EvalProbe>(json).Get();

				if (probe != null) {
					EntityContainer container = new EntityContainer();
					container.entityFields.Add(new EntityField("msus_status", "100000000"));
					containers.entityContainers.Add(container);
					MyCrm.Responses.Response response = MyCrm.Content.UpdateEntity(probe.msus_evaluationprobeid
						, "msus_evaluationprobe", container, MATT_USERID, null, proxy);

					if (response.WasSuccessful) {
						Console.WriteLine("Updated status in CRM.  Adding note...");
						MyCrm.Containers.Note note = new MyCrm.Containers.Note();
						note.notetext = "Scrapped in Jira: " + issue.key;
						note.subject = "Note";
						note.objectidtypecode = "msus_evaluationprobe";
						note.objectid = probe.msus_evaluationprobeid;
						note.owneridtype = "systemuser";
						note.ownerid = MATT_USERID;

						string noteid = MyCrm.Content.AttachNote(note, proxy);
						if (noteid != null) {
							Console.WriteLine("Note added.");
						}
					}
				} else {
					Console.WriteLine("Probe wasn't found in CRM, moving on.");
				}
			}
		}

		public class Issue {
			public string key { get; set; }
			public string PN { get; set; }
			public int SN { get; set; }
		}

		public class CrmProbes {
			public string context { get; set; }
			public List<CrmProbe> value { get; set; }
		}

		public class CrmProbe {
			public string etag { get; set; }
			public string msus_evaluationprobeid { get; set; }
			public string msus_name { get; set; }
			public string _msus_part_valueFormattedValue { get; set; }
			public string _msus_part_value { get; set; }
			public string msus_serialnumberFormattedValue { get; set; }
			public int msus_serialnumber { get; set; }
			public string _ownerid_valueFormattedValue { get; set; }
			public string _ownerid_value { get; set; }
		}

		public class JiraParts {
			public string context { get; set; }
			public List<JiraPart> value { get; set; }
		}

		public class JiraPart {
			public string msus_name { get; set; }
		}

		public class Probe {
			public string etag { get; set; }
			public string msus_evaluationprobeid { get; set; }
		}

		public class EvalProbe {
			public string context { get; set; }
			public List<Probe> value { get; set; }
			public Probe Get() {
				if (value != null && value.Count > 0) {
					return value[0];
				} else {
					return null;
				}
			}
		}

	}
}
