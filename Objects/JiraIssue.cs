using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CrmToJira {

	public class JiraIssue {
		public string expand { get; set; }
		public string id { get; set; }
		public string self { get; set; }
		public string key { get; set; }
		public Fields fields { get; set; } = new Fields();
	}



	public class Fields {

		/// <summary>
		/// Compares the fields values from this issuse with that of another.
		/// </summary>
		/// <param name="issue">A JiraIssue to compare to</param>
		/// <returns></returns>
		public bool IsEqualTo(JiraIssue issue) {
			bool isEqual = true;
			isEqual = (GetSummary() == issue.fields.GetSummary());
			if (isEqual == false) { return false; }
			isEqual = (GetDescription() == issue.fields.GetDescription());
			if (isEqual == false) { return false; }
			isEqual = (GetProject() == issue.fields.GetProject());
			if (isEqual == false) { return false; }
			isEqual = (GetIssueType() == issue.fields.GetIssueType());
			if (isEqual == false) { return false; }
			/*  isEqual = (GetPartType() == issue.fields.GetPartType());
				if (isEqual == false) { return false; }
				isEqual = (GetSystemPart() == issue.fields.GetSystemPart());
				if (isEqual == false) { return false; }  */
			isEqual = (GetProbePart() == issue.fields.GetProbePart());
			if (isEqual == false) { return false; }
			isEqual = (GetProbeSymptom() == issue.fields.GetProbeSymptom());
			if (isEqual == false) { return false; }
			isEqual = (GetProbeUsedOnSystem() == issue.fields.GetProbeUsedOnSystem());
			if (isEqual == false) { return false; }
			isEqual = (GetWarranty() == issue.fields.GetWarranty());
			if (isEqual == false) { return false; }
			isEqual = (GetMDR() == issue.fields.GetMDR());
			if (isEqual == false) { return false; }
			isEqual = (GetUsedOnSerialNumber() == issue.fields.GetUsedOnSerialNumber());
			if (isEqual == false) { return false; }
			isEqual = (GetPartNumber() == issue.fields.GetPartNumber());
			if (isEqual == false) { return false; }
			isEqual = (GetResolution() == issue.fields.GetResolution());
			if (isEqual == false) { return false; }
			isEqual = (GetSoftwareType() == issue.fields.GetSoftwareType());
			if (isEqual == false) { return false; }
			isEqual = (GetSoftwareRevision() == issue.fields.GetSoftwareRevision());
			if (isEqual == false) { return false; }
			isEqual = (GetRiskAssessment() == issue.fields.GetRiskAssessment());
			if (isEqual == false) { return false; }
			isEqual = (GetProbeType() == issue.fields.GetProbeType());
			if (isEqual == false) { return false; }
			isEqual = (GetSalesRep() == issue.fields.GetSalesRep());
			if (isEqual == false) { return false; }
			isEqual = (GetProbeRevision() == issue.fields.GetProbeRevision());
			if (isEqual == false) { return false; }
			isEqual = (GetUseCount() == issue.fields.GetUseCount());
			if (isEqual == false) { return false; }
			isEqual = (GetSerialNumber() == issue.fields.GetSerialNumber());
			if (isEqual == false) { return false; }
			isEqual = (GetImmediateAction() == issue.fields.GetImmediateAction());
			if (isEqual == false) { return false; }
			isEqual = (GetSystemType() == issue.fields.GetSystemType());
			if (isEqual == false) { return false; }
			isEqual = (GetRootCause() == issue.fields.GetRootCause());
			if (isEqual == false) { return false; }
			return isEqual;
		} // END IS EQUAL

		#region NICE GETTERS AND SETTERS
		public string GetSummary() {
			return summary;
		}

		public void SetSummary(string value) {
			summary = value;
		}
		

		public string GetDescription() {
			return description;
		}

		public void SetDescription(string value) {
			description = value;
		}

		/// <summary>
		/// CC or SR
		/// </summary>
		public string GetProject() {
			try {
				return project.key;
			} catch (Exception) {
				return null;
			}
		}

		public void SetProject(string value) {
			project = new Project();
			project.projectCategory = new ProjectCategory();
			project.projectCategory.name = "Customer Issues";
			project.key = value;
		}

		public String GetRootCause() {
			try {
				return customfield_12421.value; 
			} catch (Exception) {
				return null;
			}
		}

		public void SetRootCause(string value) {
			if (customfield_12421 == null || customfield_12421.value == null) {
				customfield_12421 = new Customfield12421();
			}
			if (value == null || value.Equals("None")) {
				customfield_12421 = null;
			} else {
				customfield_12421.value = value;
			}
		}

		/// <summary>
		/// System or Probe
		/// </summary>
		public string GetIssueType() {
			try {
				return issuetype.name;
			} catch (Exception) {
				return null;
			}
		}

		public void SetIssueType(string value) {
			issuetype = new Issuetype();
			issuetype.name = value;
		}

		/// <summary>
		/// System, MiraQ or Probe
		/// </summary>
		public string GetPartType() {
			try {
				return PartType;
			} catch (Exception) {
				return null;
			}
		}

		public void SetPartType(string value) {
			PartType = value;
		}

		public string GetSystemPart() {
			try {
				return customfield_10085.value;
			} catch (Exception) {
				return null;
			}
		}

		public void SetSystemPart(string value) {
			customfield_10085 = new Customfield10085();
			customfield_10085.value = value;
		}

		public string GetProbePart() {
			try {
				return customfield_10270.value;
			} catch (Exception) {
				return null;
			}
		}

		public void SetProbePart(string value) {
			customfield_10270 = new Customfield10270();
			customfield_10270.value = value;
		}

		public string GetProbeSymptom() {
			try {
				return customfield_10271.value;
			} catch (Exception) {
				return null;
			}
		}

		public void SetProbeSymptom(string value) {
			customfield_10271 = new Customfield10271();
			customfield_10271.value = value;
		}

		public string GetProbeUsedOnSystem() {
			try {
				return customfield_10272.value;
			} catch (Exception) {
				return null;
			}
		}

		public void SetProbeUsedOnSystem(string value) {
			customfield_10272 = new Customfield10272();
			customfield_10272.value = value;
		}

		public string GetWarranty() {
			try {
				return customfield_12420.value;
			} catch (Exception) {
				return null;
			}
		}

		public void SetWarranty(string value) {
			customfield_12420 = new Customfield12420();
			customfield_12420.value = value;
		}

		public string GetMDR() {
			try {
				return customfield_10076.value;
			} catch (Exception) {
				return null;
			}
		}

		public void SetMDR(string value) {
			customfield_10076 = new Customfield10076();
			customfield_10076.value = value;
		}

		public float GetUsedOnSerialNumber() {
			return customfield_10274;
		}

		public void SetUsedOnSerialNumber(float value) {
			customfield_10274 = value;
		}

		public string GetPartNumber() {
			try {
				return customfield_10077.child.value;
			} catch (Exception) {
				return null;
			}
		}

		public void SetPartNumber(string value) {
			customfield_10077 = new Customfield10077();
			customfield_10077.child = new Child2();
			customfield_10077.child.value = value;
			customfield_10077.value = GetPartType();
		}

		/// <summary>
		/// Note that there is not a SetResolution as this value cannot be set 
		/// during a create or update of a Jira issue.  It can only be set when 
		/// resolving an issue.
		/// </summary>
		/// <returns></returns>
		public string GetResolution() {
			if (resolution != null) {
				return resolution.name;
			} else {
				return null;
			}
		}

		public void SetResolution(string text) {
			resolution = new Resolution();
			resolution.name = text;
		}

		public string GetSoftwareType() {
			try {
				return customfield_10110.value;
			} catch (Exception) {
				return null;
			}
		}

		public void SetSoftwareType(string value) {
			customfield_10110 = new Customfield10110();
			customfield_10110.value = value;
		}

		public string GetSoftwareRevision() {
			try {
				return customfield_10110.child.value;
			} catch (Exception) {
				return null;
			}
		}

		public void SetSoftwareRevision(string value) {
			if (customfield_10110 == null) {
				customfield_10110 = new Customfield10110();
			}
			customfield_10110.value = GetSoftwareType();
			customfield_10110.child = new Child3();
			customfield_10110.child.value = value;
		}

		public string GetRiskAssessment() {
			try {
				return customfield_11520.value;
			}
			catch (Exception) {
				return null;
			}
		}

		public void SetRiskAssessment() {
			customfield_11520 = new Customfield11520();
			customfield_11520.id = "15304";
		}

		public string GetProbeType() {
			try {
				return customfield_10079.value;
			} catch (Exception) {
				return null;
			}
		}

		public void SetProbeType(string value) { 
			customfield_10079 = new Customfield10079();
			customfield_10079.value = value;
		}

		public string GetAssignee() {
			return assignee.name;
		}

		public void SetAssignee(string value) {
			assignee = new Assignee();
			assignee.name = value;
		}

		public string GetCreator() {
			return creator.key;
		}

		public void SetCreator(string value) {
			creator = new Creator();
			creator.key = value;
		}

		public string GetReporter() {
			return reporter.key;
		}

		public void SetReporter(string value) {
			reporter = new Reporter();
			reporter.key = value;
		}

		public string GetSalesRep() {
			return customfield_10140;
		}

		public void SetSalesRep(string value) {
			customfield_10140 = value;
		}

		/// <summary>
		/// Open or Closed
		/// </summary>
		public string GetStatus() {
			try {
				return status.name;
			} catch (Exception) {
				return null;
			}
		}

		public void SetStatus(string value) {
			status = new Status();
			status.name = value;
		}

		public string GetProbeRevision() {
			try {
				return customfield_10080.value;
			} catch (Exception) {
				return null;
			}
		}

		public void SetProbeRevision(string value) {
			customfield_10080 = new Customfield10080();
			customfield_10080.value = value;
		}

		public double GetUseCount() {
			return customfield_10086;
		}

		public void SetUseCount(double value) {
			customfield_10086 = value;
		}

		public double GetSerialNumber() {
			return customfield_10081;
		}

		public void SetSerialNumber(double value) {
			customfield_10081 = value;
		}

		public string GetImmediateAction() {
			return customfield_10070;
		}

		public void SetImmediateAction(string value) {
			customfield_10070 = value;
		}

		public string GetDateOfFirstUse() {
			return customfield_10075;
		}

		public void SetDateOfFirstUse(DateTime value) {
			string val = value.ToString("o");
			customfield_10075 = val;
		}

		public string GetSystemType() {
			try {
				return customfield_10084.value;
			} catch (Exception) {
				return null;
			}
		}

		public void SetSystemType(string value) {
			customfield_10084 = new Customfield10084();
			customfield_10084.value = value;
		}
		#endregion

		#region RAW GETTERS AND SETTERS
		// NOT USED IN JSON
		[Newtonsoft.Json.JsonIgnore]
		public string PartType { get; set; }
		[Newtonsoft.Json.JsonIgnore]
		public string IssueStatus { get; set; }
		/*[Newtonsoft.Json.JsonIgnore]
		public string Resolution { get; set; }*/

		public Issuetype issuetype { get; set; }
		public string customfield_10070 { get; set; }
		public object customfield_10071 { get; set; }
		public object customfield_10072 { get; set; }
		public object timespent { get; set; }
		public float customfield_10274 { get; set; }
		public Customfield10073 customfield_10073 { get; set; }
		public object customfield_11120 { get; set; }
		public Project project { get; set; }
		public Customfield10076 customfield_10076 { get; set; }
		public object customfield_11121 { get; set; }
		public List<object> fixVersions { get; set; }
		public Customfield10077 customfield_10077 { get; set; }
		public Customfield10110 customfield_10110 { get; set; }
		public Customfield11520 customfield_11520 { get; set; }
		public object aggregatetimespent { get; set; }
		public Resolution resolution { get; set; }
		public string customfield_10621 { get; set; }
		public object customfield_10622 { get; set; }
		public object resolutiondate { get; set; }
		public int workratio { get; set; }
		public DateTime lastViewed { get; set; }
		public Watches watches { get; set; }
		public DateTime created { get; set; }
		public string customfield_10075 { get; set; }
		public string customfield_10140 { get; set; }
		public object customfield_12121 { get; set; }
		public object customfield_12120 { get; set; }
		public object customfield_12123 { get; set; }
		public object customfield_10340 { get; set; }
		public object customfield_12122 { get; set; }
		public Priority priority { get; set; }
		public object customfield_10420 { get; set; }
		public object customfield_10421 { get; set; }
		public string customfield_10620 { get; set; }
		public List<object> labels { get; set; }
		public object customfield_11821 { get; set; }
		public object customfield_11820 { get; set; }
		public object aggregatetimeoriginalestimate { get; set; }
		public object timeestimate { get; set; }
		public List<object> issuelinks { get; set; }
		public Assignee assignee { get; set; }
		public DateTime updated { get; set; }
		public Status status { get; set; }
		public object timeoriginalestimate { get; set; }
		public string description { get; set; }
		public object customfield_11220 { get; set; }
		public object customfield_11221 { get; set; }
		public Timetracking timetracking { get; set; }
		public object customfield_10920 { get; set; }
		public object customfield_10921 { get; set; }
		public List<Attachment> attachment { get; set; }
		public object aggregatetimeestimate { get; set; }
		public string summary { get; set; }
		public Creator creator { get; set; }
		public double customfield_10081 { get; set; }
		public Customfield10270 customfield_10270 { get; set; }
		public List<object> subtasks { get; set; }
		public string customfield_12020 { get; set; }
		public Customfield10084 customfield_10084 { get; set; }
		public Customfield10085 customfield_10085 { get; set; }
		public object customfield_12021 { get; set; }
		public Reporter reporter { get; set; }
		public Customfield12420 customfield_12420 { get; set; }
		public Aggregateprogress aggregateprogress { get; set; }
		public string customfield_10320 { get; set; }
		public string customfield_10321 { get; set; }
		public string customfield_12025 { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Include, DefaultValueHandling = DefaultValueHandling.Ignore)]
		public Customfield12421 customfield_12421 { get; set; } = new Customfield12421();

		public string customfield_10322 { get; set; }
		public object duedate { get; set; }
		public Progress progress { get; set; }
		public Comment comment { get; set; }
		public Votes votes { get; set; }
		public Worklog worklog { get; set; }
		public Customfield10271 customfield_10271 { get; set; }
		public Customfield10272 customfield_10272 { get; set; }
		public double customfield_10086 { get; set; }
		public Customfield10080 customfield_10080 { get; set; }
		public Customfield10079 customfield_10079 { get; set; }
	}
	#endregion

	#region OBJECTS
	public class Resolution {
		public string self { get; set; }
		public string id { get; set; }
		public string description { get; set; }
		public string name { get; set; }
	}

	public class Attachment {
		public string self { get; set; }
		public string id { get; set; }
		public string filename { get; set; }
		public Author author { get; set; }
		public DateTime created { get; set; }
		public int size { get; set; }
		public string mimeType { get; set; }
		public string content { get; set; }
		public string thumbnail { get; set; }
	}

	public class Customfield10270 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Issuetype {
		public string self { get; set; }
		public string id { get; set; }
		public string description { get; set; }
		public string iconUrl { get; set; }
		public string name { get; set; }
		public bool subtask { get; set; }
	}

	public class Child {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Customfield10073 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
		public Child child { get; set; }
	}

	public class AvatarUrls {
		public string __invalid_name__48x48 { get; set; }
		public string __invalid_name__24x24 { get; set; }
		public string __invalid_name__16x16 { get; set; }
		public string __invalid_name__32x32 { get; set; }
	}

	public class ProjectCategory {
		public string self { get; set; }
		public string id { get; set; }
		public string name { get; set; }
	}

	public class Project {
		public string self { get; set; }
		public string id { get; set; }
		public string key { get; set; }
		public string name { get; set; }
		public AvatarUrls avatarUrls { get; set; }
		public ProjectCategory projectCategory { get; set; }
	}

	public class Customfield10076 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Child2 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Customfield10077 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
		public Child2 child { get; set; }
	}

	public class Child3 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Customfield10110 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
		public Child3 child { get; set; }
	}

	public class Customfield11520 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Customfield10075 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Watches {
		public string self { get; set; }
		public int watchCount { get; set; }
		public bool isWatching { get; set; }
	}

	public class Priority {
		public string self { get; set; }
		public string iconUrl { get; set; }
		public string name { get; set; }
		public string id { get; set; }
	}

	public class AvatarUrls2 {
		public string __invalid_name__48x48 { get; set; }
		public string __invalid_name__24x24 { get; set; }
		public string __invalid_name__16x16 { get; set; }
		public string __invalid_name__32x32 { get; set; }
	}

	public class Assignee {
		public string self { get; set; }
		public string name { get; set; }
		public string key { get; set; }
		public string emailAddress { get; set; }
		public AvatarUrls2 avatarUrls { get; set; }
		public string displayName { get; set; }
		public bool active { get; set; }
		public string timeZone { get; set; }
	}

	public class StatusCategory {
		public string self { get; set; }
		public int id { get; set; }
		public string key { get; set; }
		public string colorName { get; set; }
		public string name { get; set; }
	}

	public class Status {
		public string self { get; set; }
		public string description { get; set; }
		public string iconUrl { get; set; }
		public string name { get; set; }
		public string id { get; set; }
		public StatusCategory statusCategory { get; set; }
	}

	public class Timetracking {
	}

	public class AvatarUrls3 {
		public string __invalid_name__48x48 { get; set; }
		public string __invalid_name__24x24 { get; set; }
		public string __invalid_name__16x16 { get; set; }
		public string __invalid_name__32x32 { get; set; }
	}

	public class Creator {
		public string self { get; set; }
		public string name { get; set; }
		public string key { get; set; }
		public string emailAddress { get; set; }
		public AvatarUrls3 avatarUrls { get; set; }
		public string displayName { get; set; }
		public bool active { get; set; }
		public string timeZone { get; set; }
	}

	public class Customfield10084 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Customfield10085 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class AvatarUrls4 {
		public string __invalid_name__48x48 { get; set; }
		public string __invalid_name__24x24 { get; set; }
		public string __invalid_name__16x16 { get; set; }
		public string __invalid_name__32x32 { get; set; }
	}

	public class Reporter {
		public string self { get; set; }
		public string name { get; set; }
		public string key { get; set; }
		public string emailAddress { get; set; }
		public AvatarUrls4 avatarUrls { get; set; }
		public string displayName { get; set; }
		public bool active { get; set; }
		public string timeZone { get; set; }
	}

	public class Customfield12420 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Aggregateprogress {
		public int progress { get; set; }
		public int total { get; set; }
	}


	public class Customfield12421 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Progress {
		public int progress { get; set; }
		public int total { get; set; }
	}

	public class AvatarUrls5 {
		public string __invalid_name__48x48 { get; set; }
		public string __invalid_name__24x24 { get; set; }
		public string __invalid_name__16x16 { get; set; }
		public string __invalid_name__32x32 { get; set; }
	}

	public class AvatarUrls6 {
		public string __invalid_name__48x48 { get; set; }
		public string __invalid_name__24x24 { get; set; }
		public string __invalid_name__16x16 { get; set; }
		public string __invalid_name__32x32 { get; set; }
	}

	public class Author {
		public string self { get; set; }
		public string name { get; set; }
		public string key { get; set; }
		public string emailAddress { get; set; }
		public AvatarUrls5 avatarUrls { get; set; }
		public string displayName { get; set; }
		public bool active { get; set; }
		public string timeZone { get; set; }
	}

	public class UpdateAuthor {
		public string self { get; set; }
		public string name { get; set; }
		public string key { get; set; }
		public string emailAddress { get; set; }
		public AvatarUrls6 avatarUrls { get; set; }
		public string displayName { get; set; }
		public bool active { get; set; }
		public string timeZone { get; set; }
	}

	public class Comment2 {
		public string self { get; set; }
		public string id { get; set; }
		public Author author { get; set; }
		public string body { get; set; }
		public UpdateAuthor updateAuthor { get; set; }
		public DateTime created { get; set; }
		public DateTime updated { get; set; }
	}

	public class Comment {
		public List<Comment2> comments { get; set; }
		public int maxResults { get; set; }
		public int total { get; set; }
		public int startAt { get; set; }
	}

	public class Votes {
		public string self { get; set; }
		public int votes { get; set; }
		public bool hasVoted { get; set; }
	}

	public class Worklog {
		public int startAt { get; set; }
		public int maxResults { get; set; }
		public int total { get; set; }
		public List<object> worklogs { get; set; }
	}

	public class Customfield10271 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Customfield10272 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Customfield10079 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Customfield10080 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}
	#endregion
	/////////////////////////////////////////////////////////////////////


}