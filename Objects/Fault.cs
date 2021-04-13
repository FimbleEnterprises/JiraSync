using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrmToJira.Objects {
	public class Fault {
		public bool WasFaulted { get; set; } = false;
		public string FaultMessage { get; set; }
		public string FaultStackTrace { get; set; }

		public void SetFault(Exception e) {
			this.WasFaulted = true;
			this.FaultMessage = e.Message;
			this.FaultStackTrace = e.StackTrace;
		}
	}
}