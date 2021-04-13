using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JIRA_API_Proxy.Objects {
	public class AvatarUrls {
		public string __invalid_name__48x48 { get; set; }
		public string __invalid_name__24x24 { get; set; }
		public string __invalid_name__16x16 { get; set; }
		public string __invalid_name__32x32 { get; set; }
	}

	public class Author {
		public string self { get; set; }
		public string name { get; set; }
		public string key { get; set; }
		public string emailAddress { get; set; }
		public AvatarUrls avatarUrls { get; set; }
		public string displayName { get; set; }
		public bool active { get; set; }
		public string timeZone { get; set; }
	}

	public class AvatarUrls2 {
		public string __invalid_name__48x48 { get; set; }
		public string __invalid_name__24x24 { get; set; }
		public string __invalid_name__16x16 { get; set; }
		public string __invalid_name__32x32 { get; set; }
	}

	public class UpdateAuthor {
		public string self { get; set; }
		public string name { get; set; }
		public string key { get; set; }
		public string emailAddress { get; set; }
		public AvatarUrls2 avatarUrls { get; set; }
		public string displayName { get; set; }
		public bool active { get; set; }
		public string timeZone { get; set; }
	}

	public class JiraCommentCreateResponse {
		public string self { get; set; }
		public string id { get; set; }
		public Author author { get; set; }
		public string body { get; set; }
		public UpdateAuthor updateAuthor { get; set; }
		public DateTime created { get; set; }
		public DateTime updated { get; set; }
	}
}