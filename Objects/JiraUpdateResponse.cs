using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrmToJira.Objects {
	public class JiraUpdateResponse {

		public JiraUpdateResponse() {
			this.Fault = new Fault();
		}
		public bool wasResolved { get; set; } = false;
		public bool wasClosed { get; set; } = false;
		public string id { get; set; }
		public string key { get; set; }
		public string IssueStatus { get; set; }
		public string Resolution { get; set; }
		public bool WasFaulted() {
			return this.Fault.WasFaulted;
		}
		public Fault Fault { get; set; }

	}
}