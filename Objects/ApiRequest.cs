using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrmToJira.Objects {
	public class ApiRequest {

		public enum ActionToPerform {
			REFRESHCRM = 0,
			GETJIRA = 1,
			CREATEJIRA = 2,
			UPDATEJIRA = 3,
			CREATECOMMENT = 4,
			UPDATECOMMENT = 5
		}

		public ActionToPerform Action { get; set; }
		public string CrmGuid { get; set; }
		public string UserId { get; set; }
		public string IssueKey { get; set; }
		public string CommentId { get; set; }
		public string CommentText { get; set; }
	}
}