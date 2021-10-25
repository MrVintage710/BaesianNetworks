using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BaesianNetworks {
	public class BaesNetwork {
		
		private Dictionary<string, BaesNode> nodeMap = new Dictionary<string, BaesNode>();

		public BaesNetwork(string filename) {
			fromFile(filename);
		}
		
		private void fromFile(string filename) {
			string file = File.ReadAllText("./BIF/" + filename);
			
		}
	}

	public enum Tokens {
		INDENTIFIER,
		LABEL,
		OPENBRACKET
	}
}