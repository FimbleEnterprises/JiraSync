using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrmToJira.Objects {
	public class JiraCreateResponse {

		public JiraCreateResponse() {
			this.Fault = new Fault();
		}

		public string id { get; set; }
		public string key { get; set; }
		public string self { get; set; }
		public Fault Fault { get; set; }
	}
}