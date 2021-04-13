using CrmToJira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JIRA_API_Proxy.Objects {
	public class JiraRefreshResponse : CrmToJira.Objects.Fault {
		/// <summary>
		/// Fields are typically skipped because they do not exist in the Jira issue.  However
		/// they could be skipped due to error as there is no sophisticated error handling.
		/// </summary>
		public int FieldsSkipped = 0;
		public JiraIssue JiraIssue { get; set; }
	}
}