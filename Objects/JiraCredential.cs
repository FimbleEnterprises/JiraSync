using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CrmToJira.Objects {
	public class JiraCredentials {
		public string Username { get; set; }
		public string Password { get; set; }

		public string ToEncoded() {
			string mergedCredentials = string.Format("{0}:{1}", this.Username, this.Password);
			byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
			return Convert.ToBase64String(byteCredentials);
		}

	}
}