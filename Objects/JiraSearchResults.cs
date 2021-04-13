using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSync.Objects {

	public class Issuetype {
		public string self { get; set; }
		public string id { get; set; }
		public string description { get; set; }
		public string iconUrl { get; set; }
		public string name { get; set; }
		public bool subtask { get; set; }
	}

	public class Customfield10270 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Customfield10271 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
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

	public class Child {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Customfield10077 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
		public Child child { get; set; }
	}

	public class Customfield10079 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Resolution {
		public string self { get; set; }
		public string id { get; set; }
		public string description { get; set; }
		public string name { get; set; }
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

	public class Aggregateprogress {
		public int progress { get; set; }
		public int total { get; set; }
	}

	public class Progress {
		public int progress { get; set; }
		public int total { get; set; }
	}

	public class Votes {
		public string self { get; set; }
		public int votes { get; set; }
		public bool hasVoted { get; set; }
	}

	public class Child2 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Customfield10110 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
		public Child2 child { get; set; }
	}

	public class Customfield10084 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Customfield11520 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Customfield12420 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Customfield12421 {
		public string self { get; set; }
		public string value { get; set; }
		public string id { get; set; }
	}

	public class Fields {
		public Issuetype issuetype { get; set; }
		public string customfield_10070 { get; set; }
		public object customfield_10071 { get; set; }
		public Customfield10270 customfield_10270 { get; set; }
		public object customfield_10072 { get; set; }
		public object timespent { get; set; }
		public Customfield10271 customfield_10271 { get; set; }
		public object customfield_10073 { get; set; }
		public object customfield_10272 { get; set; }
		public string customfield_10075 { get; set; }
		public object customfield_11120 { get; set; }
		public Project project { get; set; }
		public object customfield_10274 { get; set; }
		public Customfield10076 customfield_10076 { get; set; }
		public object customfield_11121 { get; set; }
		public List<object> fixVersions { get; set; }
		public Customfield10077 customfield_10077 { get; set; }
		public object customfield_10078 { get; set; }
		public object aggregatetimespent { get; set; }
		public Customfield10079 customfield_10079 { get; set; }
		public Resolution resolution { get; set; }
		public string customfield_10621 { get; set; }
		public object customfield_10622 { get; set; }
		public DateTime resolutiondate { get; set; }
		public int workratio { get; set; }
		public DateTime? lastViewed { get; set; }
		public Watches watches { get; set; }
		public DateTime created { get; set; }
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
		public object customfield_10920 { get; set; }
		public object customfield_10921 { get; set; }
		public object aggregatetimeestimate { get; set; }
		public string summary { get; set; }
		public Creator creator { get; set; }
		public object customfield_10080 { get; set; }
		public double customfield_10081 { get; set; }
		public object customfield_10082 { get; set; }
		public List<object> subtasks { get; set; }
		public string customfield_12020 { get; set; }
		public double customfield_10086 { get; set; }
		public object customfield_12021 { get; set; }
		public Reporter reporter { get; set; }
		public Aggregateprogress aggregateprogress { get; set; }
		public string customfield_10320 { get; set; }
		public string customfield_10321 { get; set; }
		public string customfield_12025 { get; set; }
		public string customfield_10322 { get; set; }
		public object customfield_10521 { get; set; }
		public object duedate { get; set; }
		public Progress progress { get; set; }
		public Votes votes { get; set; }
		public Customfield10110 customfield_10110 { get; set; }
		public object customfield_11821 { get; set; }
		public object customfield_11820 { get; set; }
		public Customfield10084 customfield_10084 { get; set; }
		public object customfield_10085 { get; set; }
		public Customfield11520 customfield_11520 { get; set; }
		public Customfield12420 customfield_12420 { get; set; }
		public Customfield12421 customfield_12421 { get; set; }
	}

	public class Issue {
		public string expand { get; set; }
		public string id { get; set; }
		public string self { get; set; }
		public string key { get; set; }
		public Fields fields { get; set; }
	}

	public class JiraSearchResults {
		public string expand { get; set; }
		public int startAt { get; set; }
		public int maxResults { get; set; }
		public int total { get; set; }
		public List<Issue> issues { get; set; }
	}
}
