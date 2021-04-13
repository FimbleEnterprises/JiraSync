using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace CrmToJira.Objects {


	public class JiraIssueStatus {

		class Update {
		}

		class Transition {
			public string id { get; set; }
		}

		class Resolution {
			public string name { get; set; }
		}

		class Fields {
			public Resolution resolution { get; set; }
		}

		class JiraTransition {
			public Update update { get; set; }
			public Transition transition { get; set; }
			public Fields fields { get; set; }
		}

		public static string GetResolvedJson(string Resolution) {
			JiraTransition jiraTransition = new JiraTransition();
			jiraTransition.transition = new Transition();
			jiraTransition.transition.id = "21";
			jiraTransition.fields = new Fields();
			jiraTransition.fields.resolution = new Resolution();
			jiraTransition.fields.resolution.name = Resolution;
			string json = JsonConvert.SerializeObject(jiraTransition);
			return json;
		}

		public static string GetClosedJson() {
			return "{\"transition\":{\"id\":\"41\"}}";
		}

		public static string GetInProgressJson() {
			JiraTransition jiraTransition = new JiraTransition();
			jiraTransition.transition = new Transition();
			jiraTransition.transition.id = "11";
			jiraTransition.fields = new Fields();
			jiraTransition.fields.resolution = new Resolution();
			jiraTransition.fields.resolution.name = "In Progress";
			string json = JsonConvert.SerializeObject(jiraTransition);
			return json;
		}
	}
}