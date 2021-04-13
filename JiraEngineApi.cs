using CrmToJira.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Client;
using JIRA_API_Proxy.Objects;
using static JIRA_API_Proxy.Objects.MyConstants;
using static MyCrm.QueryFactory;
using MyCrm;
using static MyCrm.QueryFactory.Filter;
using Flurl.Http;
using System.Threading;
using MyCrm.Containers;
using MyCrm.Responses;
using Note = JIRA_API_Proxy.Objects.Note;
using static JiraSync.Logging.LoggingAndErrorHandling;
using System.Configuration;
using System.Drawing;
using JiraSync.Objects;

namespace CrmToJira {
	public class JiraApi {

		// Properties

		public static string CRM_BASE_URL { get; set; } = "https://crmauth.medistim.com/";
		public static string JIRA_BASE_URL { get; set; } = "http://gusvxd3e001:8090/rest/api/2/";
		public static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
		{
			DefaultValueHandling = DefaultValueHandling.Ignore,
			NullValueHandling = NullValueHandling.Ignore
		};

		OrganizationServiceProxy Proxy;
		Guid UserId { get; set; }
		string UserEmail { get; set; }
		JiraCredentials Credentials { get; set; }

		/// <summary>
		/// Constructs a new JiraApi object.  A proxy will be created for the object and reused in all of its methods.
		/// </summary>
		/// <param name="userid">The userid to perform actions as</param>
		public JiraApi(Guid userid) {
			this.UserId = userid;
			this.Proxy = MyCrm.Connector.GetProxy();
			Credentials = GetCredentials(this.UserId);
		}

		/// <summary>
		/// Retrieves the CRM columns of interest in a Jira issue;
		/// </summary>
		/// <returns>A ColumnSet</returns>
		public ColumnSet GetCrmJiraColumns() {
			ColumnSet set = new ColumnSet();
			set.AddColumn("msus_name");
			set.AddColumn("msus_immediateaction");
			set.AddColumn("msus_ccsr");
			set.AddColumn("msus_issuetype");
			set.AddColumn("msus_parttype");
			set.AddColumn("msus_mdrreportable");
			set.AddColumn("msus_riskassessment");
			set.AddColumn("msus_probesalesrep");
			set.AddColumn("msus_jirapartnumber");
			set.AddColumn("msus_serialnumber");
			set.AddColumn("msus_warrantee");
			set.AddColumn("msus_probetype");
			set.AddColumn("msus_systemtype");
			set.AddColumn("msus_probesymptom");
			set.AddColumn("msus_probepart");
			set.AddColumn("msus_proberevision");
			set.AddColumn("msus_probedescription");
			set.AddColumn("msus_systemdescription");
			set.AddColumn("msus_dateoffirstuse");
			set.AddColumn("msus_probeusedonsystem");
			set.AddColumn("msus_usedonsystemsn");
			set.AddColumn("msus_usecount");
			set.AddColumn("msus_systempart");
			set.AddColumn("msus_systemsoftwaretype");
			set.AddColumn("msus_softwarerevision");
			set.AddColumn("msus_status");
			set.AddColumn("msus_resolution");
			set.AddColumn("msus_rootcause");
			return set;
		}

		/// <summary>
		/// Returns a single 
		/// </summary>
		/// <param name="jiranumber"></param>
		/// <returns></returns>
		public CrmJiraIssuesLite GetCrmJiraIssue(string jiranumber) {
			if (Proxy == null) {
				try {
					Console.WriteLine("Connecting to server...", Color.ForestGreen);
					Proxy = Connector.GetProxy();
					Console.WriteLine("Connected", Color.ForestGreen);
				} catch (Exception e) {
					SendMail("3ECF2393-C71D-E711-80D2-005056A36B9B", "CRM Failed obtain the Proxy",
						"ERROR:\n" + e.Message + "\n\nStack:\n" + e.StackTrace, Proxy);
					Console.WriteLine("Failed to connect", Color.Red);
					return null;
				}
			}

			QueryFactory factory = new MyCrm.QueryFactory("msus_jiraissue");
			CrmToJira.JiraApi api = new CrmToJira.JiraApi(new Guid(DEFAULT_USERID));
			ColumnSet columns = api.GetCrmJiraColumns();
			factory.addColumn("msus_jiraissueid");
			factory.addColumn("ownerid");
			factory.addColumn("msus_jiranumber");
			factory.addColumn("modifiedon");
			QueryFactory.Filter filter = new QueryFactory.Filter();
			filter.addCondition(new Filter.FilterCondition("msus_jiranumber", Filter.Operator.EQUALS, jiranumber));
			factory.setFilter(filter);
			string query = factory.construct();
			Console.WriteLine("Looking up crm jira issue (" + jiranumber + ")...", Color.Blue);

			string results = MyCrm.Content.Retrieve(query, Proxy);
			CrmJiraIssuesLite crmissue = JsonConvert.DeserializeObject<CrmJiraIssuesLite>(results);
			if (crmissue != null) {
				Console.WriteLine("Found issue.", Color.Blue);
			}
			return crmissue;
		}

		/// <summary>
		/// Returns limited columns from CRM for the last six months but one of them is the modifiedon date.  
		/// Good for comparing the dates between the CRM and Jira versions of issues.
		/// </summary>
		/// <returns></returns>
		public CrmJiraIssuesLite GetCrmJiraIssuesModifiedDate() {
			if (Proxy == null) {
				try {
					Console.WriteLine("Connecting to server...", Color.ForestGreen);
					Proxy = Connector.GetProxy();
					Console.WriteLine("Connected", Color.ForestGreen);
				} catch (Exception e) {
					SendMail("3ECF2393-C71D-E711-80D2-005056A36B9B", "CRM Failed obtain the Proxy",
						"ERROR:\n" + e.Message + "\n\nStack:\n" + e.StackTrace, Proxy);
					Console.WriteLine("Failed to connect", Color.Red);
					return null;
				}
			}

			QueryFactory factory = new MyCrm.QueryFactory("msus_jiraissue");
			CrmToJira.JiraApi api = new CrmToJira.JiraApi(new Guid(DEFAULT_USERID));
			ColumnSet columns = api.GetCrmJiraColumns();
			factory.addColumn("msus_jiraissueid");
			factory.addColumn("ownerid");
			factory.addColumn("msus_jiranumber");
			factory.addColumn("modifiedon");
			factory.addColumn("msus_name");
			QueryFactory.Filter filter = new QueryFactory.Filter(FilterType.AND);
			filter.addCondition(new Filter.FilterCondition("modifiedon", Filter.Operator.LAST_X_MONTHS, "6"));
			filter.addCondition(new Filter.FilterCondition("msus_jiranumber", Filter.Operator.CONTAINS_DATA));
			factory.setFilter(filter);
			string query = factory.construct();
			Console.WriteLine("Getting CRM issues modified within the last six months...", Color.ForestGreen);

			string results = MyCrm.Content.Retrieve(query, Proxy);
			CrmJiraIssuesLite issues = JsonConvert.DeserializeObject<CrmJiraIssuesLite>(results);

			CrmJiraIssuesLite issues2 = new CrmJiraIssuesLite();
			issues2.value = new List<CrmIssue>();

			if (issues != null && issues.value != null && issues.value.Count > 0) {
				// Loop through and don't return issues starting with "Copy of" as that is what 
				// they will start with if the user uses the copy issue workflow.
				foreach (CrmIssue crm in issues.value) {
					if (!crm.msus_name.StartsWith("Copy of")) {
						issues2.value.Add(crm);
					} else {
						Console.WriteLine("Found a, \"Copy of\" issue.  Ignoring it.", Color.BurlyWood);
					}
				}
			}

			Console.WriteLine("Found " + issues2.value.Count + " issues.", Color.Blue);
			return issues2;
		}

		/// <summary>
		/// Updates a CRM-side jira issue using the current Jira-side values
		/// </summary>
		/// <param name="issueid">The CRM-side issue's guid</param>
		/// <param name="jirakey">The Jira issue's key (e.g. SR-11045)</param>
		/// <param name="userid">The user's id to make the request with</param>
		/// <returns></returns>
		public JiraRefreshResponse RefreshCrmFromJira(string issueid, string jirakey, string userid) {

			EntityMetadata metadata = MyCrm.Metadata.GetEntityMetadata("msus_jiraissue", Proxy);
			JiraRefreshResponse response = new JiraRefreshResponse();
			Guid guid = new Guid();
			EntityReference reference = new EntityReference();
			string strval;
			DateTime dtval;
			JiraIssue jiraIssue = new JiraIssue();
			ColumnSet columns = new ColumnSet();
			columns.AddColumn("");
			Entity crmIssue = Proxy.Retrieve("msus_jiraissue", new Guid(issueid), GetCrmJiraColumns());
			Entity OriginalEntity = crmIssue;

			// Clear existing values in case the issue type or project type has changed which
			// will screw up future updates if non-valid values are present for the project or issue type.
			AttributeCollection attributes = crmIssue.Attributes;
			List<string> keys = new List<string>();
			foreach (KeyValuePair<String, Object> attribute in attributes) {
				keys.Add(attribute.Key);
			}

			foreach (string key in keys) {
				if (key.StartsWith("msus")) {
					if (
						!key.StartsWith("msus_trigger") &&
						!key.StartsWith("msus_jiraissueid") &&
						!key.StartsWith("msus_name")
						) {
						crmIssue.Attributes[key] = null;
					}
				}
			}

			try {
				jiraIssue = GetIssue(jirakey);
				response.JiraIssue = jiraIssue;
			} catch (Exception exception) {
				response.SetFault(exception);
			}

			#region PROPERTIES

			try {
				strval = jiraIssue.fields.GetSummary();
				if (strval != null) {
					crmIssue.Attributes["msus_name"] = strval;
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				strval = jiraIssue.fields.GetImmediateAction();
				if (strval != null) {
					crmIssue.Attributes["msus_immediateaction"] = strval;
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetProject() != null) {
					crmIssue.Attributes["msus_ccsr"] = new OptionSetValue(GetOptionSetValue("msus_jiraissue", "msus_ccsr",
						jiraIssue.fields.GetProject(), Proxy));
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetIssueType() != null) {
					crmIssue.Attributes["msus_issuetype"] = new OptionSetValue(GetOptionSetValue("msus_jiraissue", "msus_issuetype",
						jiraIssue.fields.GetIssueType(), Proxy));
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetPartType() != null) {
					crmIssue.Attributes["msus_parttype"] = new OptionSetValue(GetOptionSetValue("msus_jiraissue", "msus_parttype",
						jiraIssue.fields.GetPartType(), Proxy));
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetMDR() != null) {
					crmIssue.Attributes["msus_mdrreportable"] = jiraIssue.fields.GetMDR().Equals("No") ? false : true;
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetRiskAssessment() != null) {
					crmIssue.Attributes["msus_riskassessment"] = new OptionSetValue(GetOptionSetValue("msus_jiraissue", "msus_riskassessment",
						jiraIssue.fields.GetRiskAssessment(), Proxy));
				}
			} catch (Exception) { }

			try {
				strval = jiraIssue.fields.GetSalesRep();
				if (strval != null) {
					// Nevermind the field name - it is used for both probes and systems
					crmIssue.Attributes["msus_probesalesrep"] = strval;
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				guid = GetEntityGuidByName(jiraIssue.fields.GetPartNumber(), "msus_jirapartnumber");
				reference = new EntityReference("msus_jirapartnumber", guid);
				crmIssue.Attributes["msus_jirapartnumber"] = reference;
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				object val = MyCrm.Metadata.ConvertToCrmDataType(metadata, "msus_serialnumber", jiraIssue.fields.GetSerialNumber().ToString());
				crmIssue.Attributes["msus_serialnumber"] = val;
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetProject().Equals("CC")) {
					if (jiraIssue.fields.GetWarranty() != null) {
						crmIssue.Attributes["msus_warrantee"] = new OptionSetValue(GetOptionSetValue("msus_jiraissue", "msus_warrantee",
							jiraIssue.fields.GetWarranty(), Proxy));
					}
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetIssueType().Equals("Probe")) {
					if (jiraIssue.fields.GetProbeType() != null) {
						crmIssue.Attributes["msus_probetype"] = new OptionSetValue(GetOptionSetValue("msus_jiraissue", "msus_probetype",
							jiraIssue.fields.GetProbeType(), Proxy));
					} else if (jiraIssue.fields.GetIssueType().Equals("System")) {
						crmIssue.Attributes["msus_systemtype"] = new OptionSetValue(GetOptionSetValue("msus_jiraissue", "msus_systemtype",
							jiraIssue.fields.GetSystemType(), Proxy));
					}
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetProbeSymptom() != null) {
					crmIssue.Attributes["msus_probesymptom"] = new OptionSetValue(GetOptionSetValue("msus_jiraissue", "msus_probesymptom",
						jiraIssue.fields.GetProbeSymptom(), Proxy));
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				guid = GetEntityGuidByName(jiraIssue.fields.GetProbePart(), "msus_jiraprobepart");
				reference = new EntityReference("msus_jiraprobepart", guid);
				crmIssue.Attributes["msus_probepart"] = reference;
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetProbeRevision() != null) {
					crmIssue.Attributes["msus_proberevision"] = new OptionSetValue(GetOptionSetValue("msus_jiraissue", "msus_proberevision",
						jiraIssue.fields.GetProbeRevision(), Proxy));
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetIssueType().Equals("Probe")) {
					crmIssue.Attributes["msus_probedescription"] = jiraIssue.fields.GetDescription();
				} else if (jiraIssue.fields.GetIssueType().Equals("System")) {
					crmIssue.Attributes["msus_systemdescription"] = jiraIssue.fields.GetDescription();
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetDateOfFirstUse() != null) {
					dtval = DateTime.Parse(jiraIssue.fields.GetDateOfFirstUse());
					if (dtval != null) {
						crmIssue.Attributes["msus_dateoffirstuse"] = dtval;
					}
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetIssueType().Equals("Probe")) {
					if (jiraIssue.fields.GetProbeUsedOnSystem() != null) {
						crmIssue.Attributes["msus_probeusedonsystem"] = new OptionSetValue(GetOptionSetValue("msus_jiraissue", "msus_probeusedonsystem",
							jiraIssue.fields.GetProbeUsedOnSystem(), Proxy));
					}
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetIssueType().Equals("Probe")) {
					object val = MyCrm.Metadata.ConvertToCrmDataType(metadata, "msus_usedonsystemsn", jiraIssue.fields.GetUsedOnSerialNumber().ToString());
					crmIssue.Attributes["msus_usedonsystemsn"] = val;
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetIssueType().Equals("Probe")) {
					object val = MyCrm.Metadata.ConvertToCrmDataType(metadata, "msus_usecount", jiraIssue.fields.GetUseCount().ToString());
					crmIssue.Attributes["msus_usecount"] = val;
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetIssueType().Equals("System")) {
					guid = GetEntityGuidByName(jiraIssue.fields.GetSystemPart(), "msus_jirasystempart");
					reference = new EntityReference("msus_jirasystempart", guid);
					crmIssue.Attributes["msus_systempart"] = reference;
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				if (jiraIssue.fields.GetIssueType().Equals("System")) {
					if (jiraIssue.fields.GetSoftwareType() != null) {
						crmIssue.Attributes["msus_systemsoftwaretype"] = new OptionSetValue(GetOptionSetValue("msus_jiraissue"
							, "msus_systemsoftwaretype", jiraIssue.fields.GetSoftwareType(), Proxy));
					}
				}
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				guid = GetEntityGuidByName(jiraIssue.fields.GetSoftwareRevision(), "msus_jirasystemsoftwarerevision");
				reference = new EntityReference("msus_jirasystemsoftwarerevision", guid);
				crmIssue.Attributes["msus_softwarerevision"] = reference;
			} catch (Exception) { response.FieldsSkipped += 1; }

			try {
				crmIssue.Attributes["msus_resolution"] = new OptionSetValue(GetOptionSetValue("msus_jiraissue", "msus_resolution",
							jiraIssue.fields.GetResolution(), Proxy));
			} catch (Exception) { response.FieldsSkipped += 1; }

			#endregion

			try {
				Proxy.Update(crmIssue);
				DeleteCrmComments(new Guid(issueid));
				CreateCrmComments(jiraIssue, issueid);
				ConvertAttachmentsToCrmNotes(jiraIssue, issueid);
			} catch (Exception e1) {
				Proxy.Update(OriginalEntity);
				response.SetFault(e1);
			}

			return response;
		}

		/// <summary>
		/// Pulls up the CRM iteration of the Jira issue, reading the fields in order to construct the JSON 
		/// that will constitute the message body of the POST (create) request to the Jira API.  This method
		/// is full featured, meaning it both creates and updates then resolves (if necessary) and closes (if necessary)
		/// the issue in Jira. 
		/// </summary>
		/// <param name="issueid">The guid of the CRM jira record</param>
		/// <returns>A create response containing the newly created Jira issue key (CC-XXXXX/SR-XXXXX)</returns>
		public JiraCreateResponse CreateJira(Guid issueid) {

			JiraCreateResponse response = new JiraCreateResponse();

			//  Create query using querybyattribute      
			QueryByAttribute querybyexpression = new QueryByAttribute("msus_jiraissue");
			querybyexpression.ColumnSet = new ColumnSet(true);
			//  Attribute to query      
			querybyexpression.Attributes.AddRange("msus_jiraissueid");
			//  Value of queried attribute to return      
			querybyexpression.Values.AddRange(issueid);

			Proxy = MyCrm.Connector.GetProxy();

			EntityCollection manyEntityies = Proxy.RetrieveMultiple(querybyexpression);
			Entity e = manyEntityies[0];

			EntityReference owner = (EntityReference)e.Attributes["ownerid"];
			JiraCredentials credentials = GetCredentials(owner.Id);
			if (credentials.Username == null) {
				credentials.Username = "mweber";
				credentials.Password = "mweber";
			}

			JiraIssue issue = new JiraIssue();
			OptionSetValue optionset;
			try {
				issue.fields.SetAssignee(credentials.Username);
				issue.fields.SetSummary(e.Attributes["msus_name"].ToString());
				
				// Set the patient harm to no by default
				issue.fields.SetPatientHarmToNo();

				// Debugging - Stipulate when the issue was detected.
				issue.fields.SetWhenIssueOcurred("Before surgery");

				if (e.Attributes.Contains("msus_ccsr")) {
					optionset = (OptionSetValue)e.Attributes["msus_ccsr"];
					issue.fields.SetProject(GetOptionSetText("msus_jiraissue", "msus_ccsr", optionset.Value, Proxy));
				}

				if (e.Attributes.Contains("msus_issuetype")) {
					optionset = (OptionSetValue)e.Attributes["msus_issuetype"];
					issue.fields.SetIssueType(GetOptionSetText("msus_jiraissue", "msus_issuetype", optionset.Value, Proxy));
				}

				if (e.Attributes.Contains("msus_mdrreportable")) {
					bool yes = (bool)e.Attributes["msus_mdrreportable"];
					string val = "No";
					if (yes) val = "Yes";
					issue.fields.SetMDR(val);
				}

				if (issue.fields.GetProject().Equals("CC")) {
					issue.fields.SetRiskAssessment();
				}

				//  Serialize the issue and send it off for creation
				string json = JsonConvert.SerializeObject(issue, serializerSettings);
				json = json.Replace(",\"customfield_12421\":{}", "");


				response = CreateIssue(json, credentials);

				// If it isn't faulted then take the newly created key and update issue with the fields
				// that were not available during creation.
				if (!response.Fault.WasFaulted) {
					try {
						EntityContainer container = new EntityContainer();
						container.entityFields = new List<EntityField>();
						container.entityFields.Add(new EntityField("msus_jiranumber", response.key));
						MyCrm.Content.UpdateEntity(issueid.ToString(), "msus_jiraissue", container, UserId.ToString(), null, Proxy);
					} catch (Exception e1) {
						SendMail(UserId.ToString(), "Error assigning jira number to CRM Jira Issue", e1.Message, Proxy);
					}
					// CreateCommentInJira(response.key, "-=| Created and synced by CRM |=-");
					JiraUpdateResponse updateResponse = UpdateJira(response.key, issueid.ToString(), credentials);
					if (updateResponse.Fault.WasFaulted) {
						response.Fault.WasFaulted = true;
					} else {
						try {
							if (updateResponse.Resolution != null) {
								if (!updateResponse.wasResolved) {
									JiraSync.Logging.LoggingAndErrorHandling
										.SendMail(owner.Id.ToString(), "Failed to resolve Jira " + issue.key, "You will need to resolve it manually.", Proxy);
									JiraSync.Logging.LoggingAndErrorHandling.LogSync(issue.key, "Failed to resolve", Proxy);
								}
								if (!updateResponse.wasClosed) {
									JiraSync.Logging.LoggingAndErrorHandling
										.SendMail(owner.Id.ToString(), "Failed to close Jira " + issue.key, "You will need to close it manually.", Proxy);
									JiraSync.Logging.LoggingAndErrorHandling.LogSync(issue.key, "Failed to close", Proxy);
								}
							}
						} catch (Exception ex) {
							JiraSync.Logging.LoggingAndErrorHandling
											.SendMail(owner.Id.ToString(), "Failed to change the status of Jira " + issue.key, "You will need to address this manually.", Proxy);
							JiraSync.Logging.LoggingAndErrorHandling.LogSync(ex.Message, ex.StackTrace, Proxy);
						}
					}
				}

				return response;

			} catch (Exception ex) {
				Fault fault = new Fault();
				fault.FaultMessage = ex.Message;
				fault.FaultStackTrace = ex.StackTrace;
				fault.WasFaulted = true;
				response.Fault = fault;
			}

			return response;
		}

		/// <summary>
		/// Pulls up the CRM iteration of the Jira issue, reading the fields in order to construct the JSON 
		/// that will constitute the message body of the PUT (update) request.  This function will both 
		/// resolve (if necessary) and close (if necessary) the issue in question.  The status of the issue
		/// in Jira does not need to be considered as this method handles all states to set the desired state.
		/// </summary>
		/// <param name="issueid">The guid of the CRM jira record</param>
		/// <returns></returns>
		public JiraUpdateResponse UpdateJira(string issuekey, string issueid, JiraCredentials credentials) {

			JiraUpdateResponse updateResponse = new JiraUpdateResponse();
			JiraIssue crmSideIssue = MakeJiraIssueFromCrm(issueid);
			JiraIssue jiraSideIssue = GetIssue(issuekey);
			string originalResolution = null;
			DateTime originalUpdated = new DateTime();

			// Strip out "updated" and "resolution" if they exist as they cannot be updated in this fashion
			if (crmSideIssue.fields.updated != null) {
				originalUpdated = crmSideIssue.fields.updated;
				crmSideIssue.fields.updated = new DateTime();
			}
			if (crmSideIssue.fields.resolution != null) {
				originalResolution = crmSideIssue.fields.resolution.name;
				crmSideIssue.fields.resolution = null;
			}

			string json = JsonConvert.SerializeObject(crmSideIssue, serializerSettings);
			try {

				// Since we actually need to send null a value from time to time as the root cause and that there is
				// no way I can find to toggle the IgnoreNullValue on a nested property we simply have to null it 
				// out and replace it from the json string after serialization.
				if (crmSideIssue.fields.GetProject().Equals("SR")) {
					crmSideIssue.fields.SetRootCause(null);
					json = JsonConvert.SerializeObject(crmSideIssue, serializerSettings);
					json = json.Replace(",\"customfield_12421\":{}", "");
				} else {
					json = JsonConvert.SerializeObject(crmSideIssue, serializerSettings);
				}

				// Check for notes and add them as comments and add attachments if they exist
				Notes notes = GetCrmComments(new Guid(issueid));
				foreach (Note note in notes.value) {

					// See if the note is an attachment
					if (note.isdocument) {
						bool attachmentExists = false;
						// See if the attachment already exists in jira
						foreach (Attachment jiraAttachment in jiraSideIssue.fields.attachment) {
							if (note.filename.Equals(jiraAttachment.filename)) {
								attachmentExists = true;
							}
						}
						// If it doesn't exist, attach it to the jira
						if (!attachmentExists) {
							Console.WriteLine("Found note with an attachment", Color.White);
							Console.WriteLine("Converting attachment...", Color.Gray);
							if (CreateAttachmentInJira(issuekey, note) != null) {
								Console.WriteLine("Success", Color.Green);
							}
						}
					} // end note has an attachment
					else { // Note does not have an attachment so we treat it as a future Jira comment
						bool commentExists = false;
						if ((note.notetext == null || note.notetext.Length == 0) && (note.subject != null && note.subject.Length > 0)) {
							note.notetext = note.subject;
							note.subject = "";
						}
						if (note.notetext != null && !note.notetext.Contains("-=| Created and synced by CRM |=-")) {

							// Check if the comment already exists in Jira using crude text matching
							foreach (Comment2 comment in jiraSideIssue.fields.comment.comments) {
								if (comment.body.Equals(note.notetext)) {
									commentExists = true;
								}
							}

							// if comment doesn't already exist, create it in Jira
							if (!commentExists) {
								Console.WriteLine("Found a note - converting it to a Jira comment...");
								// Found a user that was putting the comment text in the subject and this caused the update to fail

								string commentid = CreateCommentInJira(issuekey, note.notetext, credentials);
								if (commentid != null) {
									MarkNoteSubjectWithJiraCommentid(note, commentid);
									Console.WriteLine("Success", Color.Green);
								} else {
									Console.WriteLine("Conversion failed - email sent and logged", Color.Red);
								}
							} // comment exists
						} // note text doesn't equal -=| Created and synced by CRM |=-
					} // Note does not have an attachment
				} // for each note

				// Actually update the Jira using the Jira API
				bool successfullyUpdatedInJira = UpdateIssue(issuekey, json, credentials);

				// Construct our update response for the caller
				updateResponse = new JiraUpdateResponse();

				// If the actual Jira update request is successfull we set the resolution accordingly
				if (successfullyUpdatedInJira) {
					crmSideIssue.fields.SetResolution(originalResolution);

					if (originalResolution != null) {
						// If the crm issue has a resolution value we can call the ResolveIssue function as it manages
						// the status' safely (reopens the issue before changing resolution etc.)
						if (ResolveIssue(issuekey, credentials, originalResolution, true)) {
							updateResponse.wasResolved = true;
							updateResponse.wasClosed = true;
						}
					} else {
						// If it doesn't have a value then we can safely call reopenIssue as it safely handles all of the 
						// states and resolutions etc.
						if (ReOpenIssue(issuekey, credentials)) {
							updateResponse.wasResolved = false;
							updateResponse.wasClosed = false;
						}
					}
					updateResponse.key = issuekey;
					updateResponse.id = issueid.ToString();
					return updateResponse;
				} else {
					return null;
				}

			} catch (Exception ex) {
				updateResponse.Fault = new Fault();
				updateResponse.Fault.FaultMessage = ex.Message;
				if (json != null) {
					updateResponse.Fault.FaultMessage = updateResponse.Fault.FaultMessage + "\n\n" + json;
				}
				updateResponse.Fault.FaultStackTrace = ex.StackTrace;
				updateResponse.Fault.WasFaulted = true;
				return updateResponse;
			}
		}


		public bool CrmAndJiraAreEqual(string issuekey, string crmGuid) {
			JiraIssue jira = GetIssue(issuekey);
			return CrmAndJiraAreEqual(jira, crmGuid);
		}

		public bool CrmAndJiraAreEqual(JiraIssue jiraIssue, string crmGuid) {
			JiraIssue crmJira = MakeJiraIssueFromCrm(crmGuid);
			bool isEqual = (crmJira.fields.IsEqualTo(jiraIssue));
			return isEqual;
		}

		/// <summary>
		/// Creates a JiraIssue object by reading the fields from a  created Jira in CRM
		/// </summary>
		/// <param name="crmguid">GUID of the CRM Jira issue</param>
		/// <returns></returns>
		public JiraIssue MakeJiraIssueFromCrm(string crmguid) {
			//  Create query using querybyattribute      
			QueryByAttribute querybyexpression = new QueryByAttribute("msus_jiraissue");
			querybyexpression.ColumnSet = new ColumnSet(true);
			//  Attribute to query      
			querybyexpression.Attributes.AddRange("msus_jiraissueid");
			//  Value of queried attribute to return      
			querybyexpression.Values.AddRange(crmguid);

			Proxy = MyCrm.Connector.GetProxy();

			EntityCollection manyEntityies = Proxy.RetrieveMultiple(querybyexpression);
			Entity e = manyEntityies[0];

			EntityReference owner = (EntityReference)e.Attributes["ownerid"];
			JiraCredentials credentials = GetCredentials(owner.Id);
			if (credentials.Username == null) {
				credentials.Username = "mweber";
				credentials.Password = "mweber";
			}

			OptionSetValue optionset;
			EntityReference reference;

			JiraIssue issue = new JiraIssue();
			StringBuilder sb = new StringBuilder();

			try {
				#region PROPERTIES
				if (e.Attributes.Contains("msus_jiranumber")) {
					sb.AppendLine("msus_jiranumber");
					issue.key = e.Attributes["msus_jiranumber"].ToString();
				}

				issue.fields.SetSummary(e.Attributes["msus_name"].ToString());

				if (e.Attributes.Contains("modifiedon")) {
					sb.AppendLine("modifiedon");
					issue.fields.updated = (DateTime)e.Attributes["modifiedon"];
				}

				if (e.Attributes.Contains("msus_immediateaction")) {
					sb.AppendLine("msus_immediateaction");
					issue.fields.SetImmediateAction(e.Attributes["msus_immediateaction"].ToString());
				}

				if (e.Attributes.Contains("msus_ccsr")) {
					sb.AppendLine("msus_ccsr");
					optionset = (OptionSetValue)e.Attributes["msus_ccsr"];
					issue.fields.SetProject(GetOptionSetText("msus_jiraissue", "msus_ccsr", optionset.Value, Proxy));
				}

				if (e.Attributes.Contains("msus_issuetype")) {
					sb.AppendLine("msus_issuetype");
					optionset = (OptionSetValue)e.Attributes["msus_issuetype"];
					issue.fields.SetIssueType(GetOptionSetText("msus_jiraissue", "msus_issuetype", optionset.Value, Proxy));
				}

				// NOT ON CREATE SCREEN AND WILL NOT BE IN JSON BUT MUST BE SET BEFORE PART NUMBER
				if (e.Attributes.Contains("msus_parttype")) {
					sb.AppendLine("msus_parttype");
					optionset = (OptionSetValue)e.Attributes["msus_parttype"];
					issue.fields.SetPartType(GetOptionSetText("msus_jiraissue", "msus_parttype", optionset.Value, Proxy));
				}

				if (e.Attributes.Contains("msus_mdrreportable")) {
					sb.AppendLine("msus_mdrreportable");
					bool yes = (bool)e.Attributes["msus_mdrreportable"];
					string val = "No";
					if (yes) val = "Yes";
					issue.fields.SetMDR(val);
				}

				if (issue.fields.GetProject().Equals("CC")) {
					if (e.Attributes.Contains("msus_rootcause")) {
						optionset = (OptionSetValue)e.Attributes["msus_rootcause"];
						issue.fields.SetRootCause(GetOptionSetText("msus_jiraissue", "msus_rootcause", optionset.Value, Proxy));
					}
				}

				if (issue.fields.GetProject().Equals("CC")) {
					issue.fields.SetRiskAssessment();
				}

				if (e.Attributes.Contains("msus_probesalesrep")) {
					sb.AppendLine("msus_probesalesrep");
					issue.fields.SetSalesRep(e.Attributes["msus_probesalesrep"].ToString());
				}

				if (e.Attributes.Contains("msus_jirapartnumber")) {
					sb.AppendLine("msus_jirapartnumber");
					reference = (EntityReference)e.Attributes["msus_jirapartnumber"];
					issue.fields.SetPartNumber(reference.Name);
				}

				if (e.Attributes.Contains("msus_serialnumber")) {
					sb.AppendLine("msus_serialnumber");
					double val = double.Parse(e.Attributes["msus_serialnumber"].ToString());
					issue.fields.SetSerialNumber(val);
				}

				if (issue.fields.GetProject().Equals("CC")) {
					if (e.Attributes.Contains("msus_warrantee")) {
						sb.AppendLine("msus_warrantee");
						optionset = (OptionSetValue)e.Attributes["msus_warrantee"];
						issue.fields.SetWarranty(GetOptionSetText("msus_jiraissue", "msus_warrantee", optionset.Value, Proxy));
					}
				}

				if (e.Attributes.Contains("msus_probetype")) {
					sb.AppendLine("msus_probetype");
					optionset = (OptionSetValue)e.Attributes["msus_probetype"];
					issue.fields.SetProbeType(GetOptionSetText("msus_jiraissue", "msus_probetype", optionset.Value, Proxy));
				}

				if (e.Attributes.Contains("msus_probesymptom")) {
					sb.AppendLine("msus_probesymptom");
					optionset = (OptionSetValue)e.Attributes["msus_probesymptom"];
					string text = GetOptionSetText("msus_jiraissue", "msus_probesymptom", optionset.Value, Proxy);
					if (!text.Equals("None")) {
						issue.fields.SetProbeSymptom(text);
					}
				}

				if (e.Attributes.Contains("msus_probepart")) {
					sb.AppendLine("msus_probepart");
					reference = (EntityReference)e.Attributes["msus_probepart"];
					issue.fields.SetProbePart(reference.Name);
				}

				if (e.Attributes.Contains("msus_proberevision")) {
					sb.AppendLine("msus_proberevision");
					optionset = (OptionSetValue)e.Attributes["msus_proberevision"];
					issue.fields.SetProbeRevision(GetOptionSetText("msus_jiraissue", "msus_proberevision", optionset.Value, Proxy));
				}

				if (e.Attributes.Contains("msus_probedescription")) {
					sb.AppendLine("msus_probedescription");
					issue.fields.SetDescription(e.Attributes["msus_probedescription"].ToString());
				}

				if (e.Attributes.Contains("msus_dateoffirstuse")) {
					sb.AppendLine("msus_dateoffirstuse");
					DateTime dateTime = (DateTime)e.Attributes["msus_dateoffirstuse"];
					issue.fields.SetDateOfFirstUse(dateTime);
				}

				if (e.Attributes.Contains("msus_probeusedonsystem")) {
					sb.AppendLine("msus_probeusedonsystem");
					optionset = (OptionSetValue)e.Attributes["msus_probeusedonsystem"];
					issue.fields.SetProbeUsedOnSystem(GetOptionSetText("msus_jiraissue", "msus_probeusedonsystem", optionset.Value, Proxy));
				}

				if (e.Attributes.Contains("msus_usedonsystemsn")) {
					sb.AppendLine("msus_usedonsystemsn");
					float val = float.Parse(e.Attributes["msus_usedonsystemsn"].ToString());
					issue.fields.SetUsedOnSerialNumber(val);
				}

				if (e.Attributes.Contains("msus_usecount")) {
					sb.AppendLine("msus_usecount");
					double val = double.Parse(e.Attributes["msus_usecount"].ToString());
					issue.fields.SetUseCount(val);
				}

				if (e.Attributes.Contains("msus_systemtype")) {
					sb.AppendLine("msus_systemtype");
					optionset = (OptionSetValue)e.Attributes["msus_systemtype"];
					issue.fields.SetSystemType(GetOptionSetText("msus_jiraissue", "msus_systemtype", optionset.Value, Proxy));
				}

				if (e.Attributes.Contains("msus_systempart")) {
					sb.AppendLine("msus_systempart");
					reference = (EntityReference)e.Attributes["msus_systempart"];
					issue.fields.SetSystemPart(reference.Name);
				}

				if (e.Attributes.Contains("msus_systemsoftwaretype")) {
					sb.AppendLine("msus_systemsoftwaretype");
					optionset = (OptionSetValue)e.Attributes["msus_systemsoftwaretype"];
					issue.fields.SetSoftwareType(GetOptionSetText("msus_jiraissue", "msus_systemsoftwaretype", optionset.Value, Proxy));
				}

				if (e.Attributes.Contains("msus_softwarerevision")) {
					sb.AppendLine("msus_softwarerevision");
					reference = (EntityReference)e.Attributes["msus_softwarerevision"];
					issue.fields.SetSoftwareRevision(reference.Name);
				}

				if (e.Attributes.Contains("msus_systemdescription")) {
					sb.AppendLine("msus_systemdescription");
					issue.fields.SetDescription(e.Attributes["msus_systemdescription"].ToString());
				}

				if (e.Attributes.Contains("msus_resolution")) {
					try {
						sb.AppendLine("msus_resolution");
						optionset = (OptionSetValue)e.Attributes["msus_resolution"];
						issue.fields.SetResolution((GetOptionSetText("msus_jiraissue", "msus_resolution", optionset.Value, Proxy)));
					} catch (Exception) { }
				}
				#endregion
				return issue;
			} catch (Exception e1) {
				string lastAttribute = "The last attribute attempted and likely caused the error is at the bottom of this list: " + sb.ToString();
				SendMail(owner.Id.ToString(), "Error converting CRM issue to Jira issue", lastAttribute, Proxy);
				return null;
			}
		}

		/// <summary>
		/// Updates the subject field of an annotation with the Jira comment id
		/// </summary>
		/// <param name="note">A note object containing at least the annotationid property</param>
		/// <param name="commentid">The jira comment id to make the subject</param>
		/// <returns></returns>
		public bool MarkNoteSubjectWithJiraCommentid(Note note, string commentid) {
			EntityContainer container = new EntityContainer();
			container.entityFields.Add(new EntityField("subject", commentid));
			Response response = MyCrm.Content.UpdateEntity(note.annotationid, "annotation", container, UserId.ToString(), null, Proxy);
			return response.WasSuccessful;
		}

		/// <summary>
		/// Gets the user's Jira username and password which are stored in the systemuser entity in CRM.
		/// </summary>
		/// <param name="userid">The user's systemuserid in CRM</param>
		/// <returns></returns>
		public JiraCredentials GetCredentials(Guid userid) {

			JiraCredentials credential = new JiraCredentials();

			//  Create query using querybyattribute      
			QueryByAttribute querybyexpression = new QueryByAttribute("systemuser");
			querybyexpression.ColumnSet = new ColumnSet("msus_jirausername", "msus_jirapassword");
			//  Attribute to query      
			querybyexpression.Attributes.AddRange("systemuserid");
			//  Value of queried attribute to return      
			querybyexpression.Values.AddRange(userid);
			EntityCollection manyEntityies = Proxy.RetrieveMultiple(querybyexpression);

			if (manyEntityies.Entities.Count > 0) {
				Entity entity = manyEntityies[0];
				credential.Username = entity.Attributes["msus_jirausername"].ToString();
				credential.Password = entity.Attributes["msus_jirapassword"].ToString();
			}

			return credential;
		}

		/// <summary>
		/// Using the JSON data constructed from the CRM version of the soon to be created Jira issue this 
		/// method actually makes the REST request against the Jira server.
		/// </summary>
		/// <param name="json">The CRM Jira issue as reqresented in JSON markup.</param>
		/// <param name="credential">The Jira credentials to use when making the request (e.g. mweber/mweber)</param>
		/// <returns></returns>
		public JiraCreateResponse CreateIssue(string json, JiraCredentials credential) {
			JiraCreateResponse jiraResponse = new JiraCreateResponse();

			try {

				// Construct the request
				HttpWebRequest newRequest = WebRequest.Create(m_BaseUrl) as HttpWebRequest;
				newRequest.ContentType = "application/json";
				newRequest.Method = "POST"; string base64Credentials = credential.ToEncoded();
				newRequest.Headers.Add("Authorization", "Basic " + base64Credentials);
				newRequest.Accept = "application/json";

				// Write the message body
				if (json != null) {
					using (StreamWriter writer = new StreamWriter(newRequest.GetRequestStream())) {
						writer.Write(json);
					}
				}

				// Make the request and get the response
				HttpWebResponse response = newRequest.GetResponse() as HttpWebResponse;
				string result = string.Empty;
				using (StreamReader reader = new StreamReader(response.GetResponseStream())) {
					result = reader.ReadToEnd();
					jiraResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<JiraCreateResponse>(result);
					reader.Close();
					response.Close();
				}

				// Clean up and return result
				newRequest = null;
				response = null;
				return jiraResponse;

			}
			catch (Exception e) {
				// MessageBox.Show(e.Message);
				jiraResponse.Fault.WasFaulted = true;
				jiraResponse.Fault.FaultMessage = e.Message;
				jiraResponse.Fault.FaultStackTrace = e.StackTrace;
				return jiraResponse;
			}
		}

		/// <summary>
		/// Using the JSON data constructed from the CRM version of the Jira issue this method will
		/// method actually makes the REST request against the Jira server to update an existing Jira issue.
		/// </summary>
		/// <param name="json">The CRM Jira issue as reqresented in JSON markup.</param>
		/// <param name="credential">The Jira credentials to use when making the request (e.g. mweber/mweber)</param>
		/// <returns></returns>
		public bool UpdateIssue(string issuekey, string json, JiraCredentials credential) {
			try {

				// Construct the request
				HttpWebRequest newRequest = WebRequest.Create(m_BaseUrl + issuekey) as HttpWebRequest;
				newRequest.ContentType = "application/json";
				newRequest.Method = "PUT";
				string base64Credentials = credential.ToEncoded();
				newRequest.Headers.Add("Authorization", "Basic " + base64Credentials);
				newRequest.Accept = "application/json";

				// Write the message body
				if (json != null) {
					using (StreamWriter writer = new StreamWriter(newRequest.GetRequestStream())) {
						writer.Write(json);
						writer.Close();
					}
				}

				// Make the request and get the response
				HttpWebResponse response = newRequest.GetResponse() as HttpWebResponse;
				response.Close();

				// Clean up and return result
				newRequest = null;
				response = null;
				return true;
			}
			catch (Exception e) {
				return false;
			}
		}

		/// <summary>
		/// If the issue is resolved or closed it will be reopened for the purposes of setting the desired resolution value.  
		/// If the Jira resolution equals the resolution value being sent from CRM it will simply close the issue or do nothing.
		/// This method actually makes the REST request against the Jira server to resolve a Jira issue.
		/// </summary>
		/// <param name="issuekey">The Jira issue key (e.g. SR-11343).</param>
		/// <param name="credentials">The Jira credentials to use when making the request (e.g. mweber/mweber)</param>
		/// <param name="resolution">The resolution text</param>
		/// <param name="closeAfterResolution">Optionally closes the issue after resolving (true by default)</param>
		/// <returns></returns>
		public bool ResolveIssue(string issuekey, JiraCredentials credentials, string resolution, bool closeAfterResolution = true) {
			JiraCreateResponse jiraResponse = new JiraCreateResponse();

			try {

				// If the issue status is "Resolved" or "Closed" we reopen it
				if (IssueStatusClosed(issuekey) || IssueStatusResolved(issuekey)) {
					ReOpenIssue(issuekey, credentials);
				}
				
				// Construct the request
				HttpWebRequest newRequest = WebRequest.Create(m_BaseUrl + issuekey + "/transitions?expand=transitions.fields&transitionId=21") as HttpWebRequest;
				newRequest.ContentType = "application/json";
				newRequest.Method = "POST";
				string base64Credentials = credentials.ToEncoded();
				newRequest.Headers.Add("Authorization", "Basic " + base64Credentials);
				newRequest.Accept = "application/json";

				string json = JiraIssueStatus.GetResolvedJson(resolution);				

				// Write the message body
				if (json != null) {
					using (StreamWriter writer = new StreamWriter(newRequest.GetRequestStream())) {
						writer.Write(json);
						writer.Close();
					}
				}

				// Make the request and get the response
				HttpWebResponse response = newRequest.GetResponse() as HttpWebResponse;

				// Clean up and return result
				response.Close();
				newRequest = null;
				response = null;

				LogSync(issuekey + " was resolved.", "", Proxy);
				Console.WriteLine(issuekey + " was resolved (" + resolution + ")");

				if (closeAfterResolution) {
					this.CloseIssue(issuekey, credentials);
				}

				return true;

			}
			catch (Exception e) {
				return false;
			}
		}

		/// <summary>
		/// This method actually makes the REST request against the Jira server to close a Jira issue.
		/// </summary>
		/// <param name="issuekey">The Jira issue key (e.g. SR-11343).</param>
		/// <param name="credential">The Jira credentials to use when making the request (e.g. mweber/mweber)</param>
		/// <returns></returns>
		public bool CloseIssue(string issuekey, JiraCredentials credential) {
			JiraCreateResponse jiraResponse = new JiraCreateResponse();

			if (IssueStatusClosed(issuekey)) {
				return true;
			}

			try {

				// Construct the request
				HttpWebRequest newRequest = WebRequest.Create(m_BaseUrl + issuekey + "/transitions") as HttpWebRequest;
				newRequest.ContentType = "application/json";
				newRequest.Method = "POST";
				string base64Credentials = credential.ToEncoded();
				newRequest.Headers.Add("Authorization", "Basic " + base64Credentials);
				newRequest.Accept = "application/json";

				string json = JiraIssueStatus.GetClosedJson();

				// Write the message body
				if (json != null) {
					using (StreamWriter writer = new StreamWriter(newRequest.GetRequestStream())) {
						writer.Write(json);
						writer.Close();
					}
				}

				// Make the request and get the response
				HttpWebResponse response = newRequest.GetResponse() as HttpWebResponse;

				// Clean up and return result
				response.Close();
				newRequest = null;
				response = null;
				LogSync(issuekey + " was closed.", "", Proxy);
				Console.WriteLine(issuekey + " was closed");
				return true;

			} catch (Exception e) {
				return false;
			}
		}

		/// <summary>
		/// Returns issue status == Open or In Progress
		/// </summary>
		/// <returns></returns>
		public bool IssueStatusOpenOrInProgress(string issuekey) {
			JiraIssue issue = GetIssueStatusAndResolution(issuekey);
			return ((issue.fields.status.name.Equals("Open") || issue.fields.status.name.Equals("In Progress")));
		}

		/// <summary>
		/// Returns issue status == Open or In Progress
		/// </summary>
		/// <returns></returns>
		public bool IssueStatusOpenOrInProgress(JiraIssue issue) {
			return ((issue.fields.status.name.Equals("Open") || issue.fields.status.name.Equals("In Progress")));
		}

		/// <summary>
		/// Returns the issue status == Resolved.  Note that an issue can have a resololution value (which cannot be null'd)
		/// while having any status.
		/// </summary>
		/// <returns></returns>
		public bool IssueStatusResolved(string issuekey) {
			JiraIssue issue = GetIssueStatusAndResolution(issuekey);
			return (issue.fields.status.name.Equals("Resolved"));
		}

		/// <summary>
		/// Returns the issue status == Resolved.  Note that an issue can have a resololution value (which cannot be null'd)
		/// while having any status.
		/// </summary>
		/// <returns></returns>
		public bool IssueStatusResolved(JiraIssue issue) {
			return (issue.fields.status.name.Equals("Resolved"));
		}

		/// <summary>
		/// Returns the issue status == Closed
		/// </summary>
		/// <returns></returns>
		public bool IssueStatusClosed(string issuekey) {
			JiraIssue issue = GetIssueStatusAndResolution(issuekey);
			return (issue.fields.status.name.Equals("Closed"));
		}

		/// <summary>
		/// Returns the issue status == Closed
		/// </summary>
		/// <returns></returns>
		public bool IssueStatusClosed(JiraIssue issue) {
			return (issue.fields.status.name.Equals("Closed"));
		}

		/// <summary>
		/// Returns the issue resolution value.  Note that an issue can have a resololution value (which cannot be re-null'd)
		/// while having any status.
		/// </summary>
		/// <returns>Returns the issue resolution value or null</returns>
		public string GetResolutionValue(string issuekey) {
			JiraIssue issue = GetIssueStatusAndResolution(issuekey);
			if (issue.fields.resolution == null || issue.fields.resolution.name == null) {
				return null;
			}
			return issue.fields.resolution.name;
		}

		/// <summary>
		/// Returns the issue resolution value.  Note that an issue can have a resololution value (which cannot be re-null'd)
		/// while having any status.
		/// </summary>
		/// <returns>Returns the issue resolution value or null</returns>
		public string GetResolutionValue(JiraIssue issue) {
			if (issue.fields.resolution == null || issue.fields.resolution.name == null) {
				return null;
			}
			return issue.fields.resolution.name;
		}

		/// <summary>
		/// This method actually makes the REST request against the Jira server to reopen a Jira issue.
		/// </summary>
		/// <param name="issuekey">The Jira issue key (e.g. SR-11343)</param>
		/// <param name="credential">The Jira credentials to use when making the request (e.g. mweber/mweber)</param>
		/// <returns></returns>
		public bool ReOpenIssue(string issuekey, JiraCredentials credential) {

			JiraIssue issue = GetIssueStatusAndResolution(issuekey);

			if (issue.fields.status.name.Equals("Open") || issue.fields.status.name.Equals("In Progress")) {
				return true;
			}

			// Remember this is the Status of the issue NOT the Resolution value
			if (IssueResolvedButNotClosed(issuekey)) {
				CloseIssue(issuekey, credential);
			}

			try {

				// Construct the request
				HttpWebRequest newRequest = WebRequest.Create(m_BaseUrl + issuekey + "/transitions/") as HttpWebRequest;
				newRequest.ContentType = "application/json";
				newRequest.Method = "POST";
				string base64Credentials = credential.ToEncoded();
				newRequest.Headers.Add("Authorization", "Basic " + base64Credentials);
				newRequest.Accept = "application/json";

				string json = "{\"transition\": { \"id\": \"51\" }}";

				// Write the message body
				if (json != null) {
					using (StreamWriter writer = new StreamWriter(newRequest.GetRequestStream())) {
						writer.Write(json);
					}
				}

				// Make the request and get the response
				HttpWebResponse response = newRequest.GetResponse() as HttpWebResponse;

				// Clean up and return result
				newRequest = null;
				response = null;
				Console.WriteLine(issuekey + " was reopened");
				return true;

			} catch (Exception e) {
				return false;
			}
		}

		/// <summary>
		/// Gets JiraIssues from the Jira server based on last updated.
		/// </summary>
		/// <param name="updatedInLastXhours">Retrieve Jira issues updated in the last x hours</param>
		/// <returns></returns>
		public JiraSearchResults SearchJira(int maxresults = 50) {

			//Make Request
			JiraSearchResults searchResults = new JiraSearchResults();
			var newRequest = (HttpWebRequest)WebRequest.Create(JIRA_BASE_URL 
				+ "search?jql=assignee+in+membersOf(\"medistim-usa\")+order+by+updated+desc&maxResults=" + maxresults + "");
			newRequest.Headers.Add("Authorization", "Basic " + Credentials.ToEncoded());
			newRequest.ContentType = "application/json";
			newRequest.Method = "GET";

			//Get Response
			var httpResponse = (HttpWebResponse)newRequest.GetResponse();
			using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
				var result = streamReader.ReadToEnd();
				if (httpResponse.StatusCode == HttpStatusCode.OK) {
					searchResults = Newtonsoft.Json.JsonConvert.DeserializeObject<JiraSearchResults>(result,
						new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore,
							DefaultValueHandling = DefaultValueHandling.Ignore
						});
				}
				streamReader.Close();
				httpResponse.Close();

				return searchResults;
			}
		}

		/// <summary>
		/// Gets a JiraIssue from the Jira server.
		/// </summary>
		/// <param name="issuename">The Jira issue key (e.g. SR-11343)</param>
		/// <param name="credential">The Jira credentials to use when making the request (e.g. mweber/mweber)</param>
		/// <returns></returns>
		public JiraIssue GetIssue(string issuename) {

			//Make Request
			JiraIssue issue = new JiraIssue();
			var newRequest = (HttpWebRequest)WebRequest.Create(m_BaseUrl + issuename);
			newRequest.Headers.Add("Authorization", "Basic " + Credentials.ToEncoded());
			newRequest.ContentType = "application/json";
			newRequest.Method = "GET";

			//Get Response
			var httpResponse = (HttpWebResponse)newRequest.GetResponse();
			using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
				var result = streamReader.ReadToEnd();
				if (httpResponse.StatusCode == HttpStatusCode.OK) {
					issue = Newtonsoft.Json.JsonConvert.DeserializeObject<JiraIssue>(result,
						new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore,
							DefaultValueHandling = DefaultValueHandling.Ignore
						});
				}
				streamReader.Close();
				httpResponse.Close();
				return issue;
			}
		}
		
		/// <summary>
		/// Gets a JiraIssue from the Jira server but only has the fields.status and fields.resolution properties populated.
		/// </summary>
		/// <param name="issuename">The Jira issue key (e.g. SR-11343)</param>
		/// <param name="credential">The Jira credentials to use when making the request (e.g. mweber/mweber)</param>
		/// <returns></returns>
		private JiraIssue GetIssueStatusAndResolution(string issuename) {

			//Make Request
			JiraIssue issue = new JiraIssue();
			var newRequest = (HttpWebRequest)WebRequest.Create(m_BaseUrl + issuename + "?fields=status,resolution");
			newRequest.Headers.Add("Authorization", "Basic " + Credentials.ToEncoded());
			newRequest.ContentType = "application/json";
			newRequest.Method = "GET";

			//Get Response
			var httpResponse = (HttpWebResponse)newRequest.GetResponse();
			using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
				var result = streamReader.ReadToEnd();
				if (httpResponse.StatusCode == HttpStatusCode.OK) {
					issue = Newtonsoft.Json.JsonConvert.DeserializeObject<JiraIssue>(result,
						new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore,
							DefaultValueHandling = DefaultValueHandling.Ignore
						});
				}
				streamReader.Close();
				httpResponse.Close();
				return issue;
			}
		}

		/// <summary>
		/// Checks if the issue is currently "Closed"
		/// </summary>
		/// <param name="issueKey"></param>
		/// <returns>True if the status is "Closed", false for any other status (including "Resolved")</returns>
		public bool IsIssueOpen(string issueKey) {
			JiraIssue issue = GetIssueStatusAndResolution(issueKey);
			return ! issue.fields.status.name.Equals("Closed");
		}

		public bool IssueResolvedButNotClosed(string issuekey) {
			JiraIssue issue = GetIssueStatusAndResolution(issuekey);
			return issue.fields.status.name.Equals("Resolved");
		}

		public bool EditCommentInJira(string issuekey, string msg, string commentid) {
			StringBuilder sb = new StringBuilder();
			sb.Append("{");
			sb.Append("\"body\": \"" + msg +"\"");
			sb.Append("}");
			string requestjson = sb.ToString();
			var newRequest = (HttpWebRequest)WebRequest.Create(m_BaseUrl + issuekey + "/comment/" + commentid);
			newRequest.Headers.Add("Authorization", "Basic " + Credentials.ToEncoded());
			newRequest.ContentType = "application/json";
			newRequest.Method = "PUT";

			using (StreamWriter writer = new StreamWriter(newRequest.GetRequestStream())) {
				writer.Write(requestjson);
			}

			//Get Response
			try {
				var httpResponse = (HttpWebResponse)newRequest.GetResponse();
				using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
					var result = streamReader.ReadToEnd();
					httpResponse.Close();
					streamReader.Close();
					return httpResponse.StatusCode == HttpStatusCode.OK;
				}
			} catch (Exception e) { return false; }
		}

		public string CreateCommentInJira(string issuekey, string msg, JiraCredentials credentials) {
			JiraCommentCreateResponse response = new JiraCommentCreateResponse();
			var newRequest = (HttpWebRequest)WebRequest.Create(m_BaseUrl + issuekey + "/comment");
			newRequest.Headers.Add("Authorization", "Basic " + credentials.ToEncoded());
			newRequest.ContentType = "application/json";
			newRequest.Method = "POST";

			JiraCommentBody body = new JiraCommentBody();
			body.body = msg;

			using (StreamWriter writer = new StreamWriter(newRequest.GetRequestStream())) {
				string comment = JsonConvert.SerializeObject(body);
				writer.Write(comment);
			}

			//Get Response
			try {
				var httpResponse = (HttpWebResponse)newRequest.GetResponse();
				using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
					var result = streamReader.ReadToEnd();
					if (httpResponse.StatusCode == HttpStatusCode.Created) {
						response = Newtonsoft.Json.JsonConvert.DeserializeObject<JiraCommentCreateResponse>(result,
							new JsonSerializerSettings
							{
								NullValueHandling = NullValueHandling.Ignore,
								DefaultValueHandling = DefaultValueHandling.Ignore
							});
						return response.id;
					}
					httpResponse.Close();
					streamReader.Close();
				}
				return null;
			} catch (Exception e) {
				SendMail(UserId.ToString(), "Failed creating comment in Jira (" + issuekey + ")", "Error:\n"
					+ e.Message + "\n\nStack:\n" + e.StackTrace, Proxy);
				LogSync(issuekey, e.Message + "\n\n" + e.StackTrace, Proxy);
				return null;
			}
		}

		/// <summary>
		/// Gets the notes from a CRM-side Jira issue.  Attachments may be present, check the, "isdocument" property.
		/// </summary>
		/// <param name="issueid">The GUID of the jira issue to find notes for.</param>
		/// <returns>A Notes object that has a List<Note> in its "value" property.</returns>
		public Notes GetCrmComments(Guid issueid) {
			MyCrm.QueryFactory factory = new QueryFactory("annotation");
			factory.addColumn("subject");
			factory.addColumn("notetext");
			factory.addColumn("filename");
			factory.addColumn("filesize");
			factory.addColumn("documentbody");
			factory.addColumn("mimetype");
			factory.addColumn("createdby");
			factory.addColumn("modifiedon");
			factory.addColumn("createdon");
			factory.addColumn("overriddencreatedon");
			factory.addColumn("isdocument");
			factory.addColumn("annotationid");
			factory.addColumn("objectid");
			FilterCondition condition = new FilterCondition("objectid", Operator.EQUALS, issueid.ToString());
			Filter filter = new Filter(FilterType.AND, condition);
			factory.setFilter(filter);

			string query = factory.construct();
			string result = MyCrm.Content.Retrieve(query, Proxy);
			Notes notes = JsonConvert.DeserializeObject<Notes>(result); 

			return notes;
		}

		/// <summary>
		/// Retrieves the comments from a Jira issue and converts them to annotations for 
		/// consumption by the Dynamics api and submits them to CRM.
		/// </summary>
		/// <param name="issue">A JiraIssue object</param>
		/// <param name="crmissueid">The CRM version of this issue's GUID</param>
		public bool CreateCrmComments(JiraIssue issue, string crmissueid) {
			Notes notes = new Notes();
			notes.value = new List<Note>();
			foreach (Comment2 comment in issue.fields.comment.comments) {
				Note note = new Note();
				note.subject = "Jira Comment";
				note.notetext = comment.body;
				note.notetext = note.notetext + "\n\n(" + comment.author.displayName + ")";
				note.overriddencreatedon = comment.created;
				note.modifiedon = comment.created;
				note.createdon = comment.created;
				note._objectid_value = crmissueid;
				note.objecttypecode = "msus_jiraissue";
				notes.value.Add(note);
			}

			try {
				EntityCollection entities = notes.ToEntities();

				ExecuteMultipleRequest executeMultipe = new ExecuteMultipleRequest()
				{
					// Assign settings that define execution behavior: continue on error, return responses.
					Settings = new ExecuteMultipleSettings()
					{
						ContinueOnError = true,
						ReturnResponses = true
					},
					// Create an empty organization request collection.
					Requests = new OrganizationRequestCollection()
				};

				foreach (Entity entity in entities.Entities) {
					CreateRequest createReq = new CreateRequest() { Target = entity };
					executeMultipe.Requests.Add(createReq);
				}

				ExecuteMultipleResponse responseWithResults = (ExecuteMultipleResponse) Proxy.Execute(executeMultipe);

				if (!responseWithResults.IsFaulted) {
					for (int i = 0; i < responseWithResults.Responses.Count; i++) {
						ExecuteMultipleResponseItem response = responseWithResults.Responses[i];
						string guid = response.Response["id"].ToString().Replace("{", "").Replace("}", "");
						Entity entity = entities.Entities[i];
						DateTime modifiedon = (DateTime)entity.Attributes["modifiedon"];
						modifiedon.AddHours(7);
						MyCrm.DatabaseDirect.UpdateModifiedOn("annotation", guid, modifiedon);
					}
				}


			} catch (Exception e) {

			}

			return true;
		}

		/// <summary>
		/// It would be too hard to sync comments so instead they shall be deleted and re-added during refreshes.
		/// </summary>
		/// <param name="issueid">The CRM-side Jira issue to kill comments in.</param>
		public bool DeleteCrmComments(Guid issueid) {
			Notes notes = new Notes();
			List<string> notesToDie = new List<string>();

			try {
				notes = GetCrmComments(issueid);
			} catch (Exception) { }

			for (int i = 0; i < notes.value.Count; i++) {
				notesToDie.Add(notes.value[i].annotationid);
			}

			Responses responses = MyCrm.Content.DeleteManyWithResult("annotation", notesToDie, UserId.ToString(), Proxy);

			return responses.ErrorMessage == null;
		}

		/// <summary>
		/// Retrieves the attachments from a Jira issue and converts them to annotations
		/// with attachments for consumption by the Dynamics api and submits them to CRM.
		/// </summary>
		/// <param name="issue">A JiraIssue object</param>
		/// <param name="crmissueid">The CRM version of this issue's GUID</param>
		public void ConvertAttachmentsToCrmNotes(JiraIssue issue, string crmissueid) {
			List<Attachment> attachments = issue.fields.attachment;
			Notes notes = new Notes();
			notes.value = new List<Note>();
			if (attachments != null && attachments.Count > 0) {
				foreach (Attachment attachment in attachments) {
					Note note = new Note(attachment, crmissueid, Credentials);
					notes.value.Add(note);
				}
				EntityCollection entities = notes.ToEntities();
				ExecuteMultipleRequest executeMultipe = new ExecuteMultipleRequest()
				{
					// Assign settings that define execution behavior: continue on error, return responses.
					Settings = new ExecuteMultipleSettings()
					{
						ContinueOnError = true,
						ReturnResponses = true
					},
					// Create an empty organization request collection.
					Requests = new OrganizationRequestCollection()
				};

				foreach (Entity entity in entities.Entities) {
					CreateRequest createReq = new CreateRequest() { Target = entity };
					executeMultipe.Requests.Add(createReq);
				}

				ExecuteMultipleResponse responseWithResults = (ExecuteMultipleResponse)Proxy.Execute(executeMultipe);

				if ( ! responseWithResults.IsFaulted) {
					for(int i = 0; i < responseWithResults.Responses.Count; i++) {
						ExecuteMultipleResponseItem response = responseWithResults.Responses[i];
						string guid = response.Response["id"].ToString().Replace("{", "").Replace("}", "");
						Entity entity = entities.Entities[i];
						DateTime modifiedon = (DateTime)entity.Attributes["modifiedon"];
						modifiedon.AddHours(7);
						MyCrm.DatabaseDirect.UpdateModifiedOn("annotation", guid, modifiedon);
					}
				}

			}
		}

		public string CreateAttachmentInJira(string issuekey, Note note) {
			try {
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				byte[] fileContents = Convert.FromBase64String(note.documentbody);
				string postUrl = "http://gusvxd3e001:8090/rest/api/latest/issue/" + issuekey + "/attachments";

				HttpClient client = new HttpClient();
				client.BaseAddress = new System.Uri(postUrl);
				client.DefaultRequestHeaders.Add("X-Atlassian-Token", "nocheck");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Credentials.ToEncoded());
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				MultipartFormDataContent content = new MultipartFormDataContent();

				//The code which solved the problem**  
				HttpContent fileContent = new ByteArrayContent(fileContents);
				fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(note.mimetype);
				content.Add(fileContent, "file", note.filename);

				var result = client.PostAsync(postUrl, content).Result;
				return result.ToString();
			} catch (Exception e) {
				SendMail(UserId.ToString(), "Failed creating attachment in Jira (" + issuekey + ")", "Error:\n"
					+ e.Message + "\n\nStack:\n" + e.StackTrace, Proxy);
				LogSync(issuekey, e.Message + "\n\n" + e.StackTrace, Proxy);
				return null;
			}
		}

		/// <summary>
		///This method will gives you the optionset text based on value 
		/// </summary>
		/// <param name="entityName"> Entity Name of otionset field </param>
		/// <param name="fieldName">Optionset field Name</param>
		/// <param name="optionSetValue"> Option Set Value</param>
		/// <param name="service">Organization Service</param>
		/// <returns></returns>
		public static string GetOptionSetText(string entityName, string fieldName, int optionSetValue, OrganizationServiceProxy service) {

			var attReq = new RetrieveAttributeRequest();
			attReq.EntityLogicalName = entityName;
			attReq.LogicalName = fieldName;
			attReq.RetrieveAsIfPublished = true;
			var attResponse = (RetrieveAttributeResponse)service.Execute(attReq);
			var attMetadata = (EnumAttributeMetadata)attResponse.AttributeMetadata;
			return attMetadata.OptionSet.Options.Where(x => x.Value == optionSetValue).FirstOrDefault().Label.UserLocalizedLabel.Label;

		}

		/// <summary>
		///This method will gives you an optionset value based on text 
		/// </summary>
		/// <param name="entityName"> Entity Name of otionset field </param>
		/// <param name="fieldName">Optionset field Name</param>
		/// <param name="optionSetText"> Option Set Text</param>
		/// <param name="service">Organization Service</param>
		/// <returns></returns>
		public static int GetOptionSetValue(string entityName, string fieldName, string optionSetText, OrganizationServiceProxy service) {

			var attReq = new RetrieveAttributeRequest();
			attReq.EntityLogicalName = entityName;
			attReq.LogicalName = fieldName;
			attReq.RetrieveAsIfPublished = true;
			var attResponse = (RetrieveAttributeResponse)service.Execute(attReq);
			var attMetadata = (EnumAttributeMetadata)attResponse.AttributeMetadata;

			OptionMetadataCollection options = attMetadata.OptionSet.Options;
			for(int i = 0; i < options.Count; i++) {
				OptionMetadata option = options[i];
				if (option.Label.UserLocalizedLabel.Label.Equals(optionSetText)) {
					return option.Value.GetValueOrDefault();
				}
			}
			return -1;
		}

		/// <summary>
		/// This method will query an entity's msus_name field looking for the supplied text and return
		/// that record's guid.
		/// </summary>
		/// <param name="value">The text value to look for</param>
		/// <param name="entityname">The entity's logical name to query</param>
		/// <returns></returns>
		public Guid GetEntityGuidByName(string value, string entityname) {
			Guid guid = new Guid();
			QueryFactory factory = new QueryFactory(entityname);
			factory.addColumn(entityname + "id");
			FilterCondition condition = new FilterCondition("msus_name", Operator.EQUALS, value);
			QueryFactory.Filter filter = new QueryFactory.Filter(FilterType.AND, condition);
			filter.addCondition(condition);
			factory.setFilter(filter);
			string query = factory.construct();
			string result = MyCrm.Content.Retrieve(query, Proxy);
			CrmQueryResponseObjects response = JsonConvert.DeserializeObject<CrmQueryResponseObjects>(result);
			if (response != null && response.value.Count > 0) {
				guid = new Guid(response.value[0].msus_jirapartnumberid);
			}
			return guid;
		}

		public static string GetTestJson() {
			JiraIssue i = new JiraIssue();

			i.fields.SetProject("CC");
			i.fields.SetSummary("Test probe summary");
			i.fields.SetDescription("test probe description");
			i.fields.SetMDR("No");
			i.fields.SetIssueType("Probe");
			i.fields.SetPartNumber("PQ100032 3MM QUICKFIT PROBE");

			// return Newtonsoft.Json.JsonConvert.SerializeObject(i);
			return Newtonsoft.Json.JsonConvert.SerializeObject(i, new JsonSerializerSettings
				{
				DefaultValueHandling = DefaultValueHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore
				}
			);
		}

	}
}