using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSync.Objects {
	 public class MyConsole  {

		private string lastline { get; set; }

		public static void WriteToSameLine(string line, Color color) {
			Console.Write("\r{0}   ", line);
		}

	}
}
