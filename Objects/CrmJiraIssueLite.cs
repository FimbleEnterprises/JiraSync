using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSync.Objects {
	public class CrmIssue {
		public string etag { get; set; }
		public string msus_jiraissueid { get; set; }
		public string _ownerid_valueFormattedValue { get; set; }
		public string _ownerid_value { get; set; }
		public string msus_jiranumber { get; set; }
		public string modifiedonFormattedValue { get; set; }
		public DateTime modifiedon { get; set; }
		public string msus_name { get; set; }
	}

	public class CrmJiraIssuesLite {
		public string context { get; set; }
		public List<CrmIssue> value { get; set; }
	}
}
