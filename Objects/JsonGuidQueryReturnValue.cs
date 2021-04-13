using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JIRA_API_Proxy.Objects {
	public class CrmQueryResponseObject {
		public string etag { get; set; }
		public string msus_jirapartnumberid { get; set; }
	}

	public class CrmQueryResponseObjects {
		public string context { get; set; }
		public List<CrmQueryResponseObject> value { get; set; }
	}
}