using CrmToJira;
using CrmToJira.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JIRA_API_Proxy.Objects {
	public class JiraGetResponse {

		Fault _fault { get; set; }
		public Fault Fault { get; set; }
		public JiraIssue JiraIssue { get; set; }

		public JiraGetResponse() {
			this.Fault = new Fault();
		}

		public void SetFault(Exception e) {
			this.Fault.FaultMessage = e.Message;
			this.Fault.FaultStackTrace = e.StackTrace;
			this.Fault.WasFaulted = true;
		}

	}
}