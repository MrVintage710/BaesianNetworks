using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BaesianNetworks {
	public class BaesNetwork {
		
		private Dictionary<string, BaesNode> nodeMap = new Dictionary<string, BaesNode>();

		public BaesNetwork(string filename) {
			string file = File.ReadAllText("./BIF/" + filename);
			tokenize(file);
		}
		
		private List<Tuple<Token, string>> tokenize(string file) {
			ParceManager manager = new ParceManager(file);
			List<Tuple<Token, string>> tokens = new List<Tuple<Token, string>>();

			bool comment = false;
			
			while (manager.hasNext()) {
				var head = manager.next();

				if (head == "//") comment = true;

				if (comment && manager.getCurrent() == '\n') {
					tokens.Add(new Tuple<Token, string>(Token.COMMENT, head));
					Console.WriteLine(head);
					comment = false;
					manager.mark();
					continue;
				} 
					

				if (!comment) {
					if(head == "{")
					if (manager.isCurrentWhitesapce()) {
						Console.WriteLine(head);
						return tokens;
					}
				}
					
			}

			return tokens;
		}
	}

	public enum Token {
		INDENTIFIER,
		LABEL,
		INTEGER,
		DECIMAL,
		OPENBRACKET,
		CLOSEDBRACKET,
		COMMENT
	}
}