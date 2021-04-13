using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSync.Objects {
	public class JiraIssueId {
		public string etag { get; set; }
		public string msus_jiraissueid { get; set; }
		public string _ownerid_value { get; set; }
		public string msus_jiranumber { get; set; }
		public string msus_name { get; set; }
		public DateTime modifiedon { get; set; }
	}

	public class JiraIssueIdContainer {
		public string context { get; set; }
		public List<JiraIssueId> value { get; set; }
	}
}
