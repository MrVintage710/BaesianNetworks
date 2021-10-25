using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace BaesianNetworks {
	public class BaesNetwork {
		
		private Dictionary<string, BaesNode> nodeMap = new Dictionary<string, BaesNode>();

		public BaesNetwork(string filename) {
			string file = File.ReadAllText("./BIF/" + filename);
			var tokens = tokenize(file);
			Console.WriteLine(string.Join(",\n", tokens));
		}
		
		private List<TokenInfo> tokenize(string file) {
			ParceManager manager = new ParceManager(file);
			List<TokenInfo> tokens = new List<TokenInfo>();

			bool comment = false;
			bool label = false;

			while (manager.hasNext()) {
				var head = manager.getHead();

				if (head == "//") comment = true;

				if (comment && manager.getCurrent() == '\n') {
					tokens.Add(new TokenInfo(Token.COMMENT, head, manager.lastIndex(), manager.currentIndex()));
					comment = false;
					manager.mark();
					continue;
				}

				if (!comment) {
					if (manager.getCurrent() == '{') {
						if (manager.headLength() > 0) tokens.Add(new TokenInfo(Token.INDENTIFIER, head.Substring(0, head.Length), manager.getLast(), manager.getCurrent() - 1));
						tokens.Add(new TokenInfo(Token.OPENBRACKET, manager.getCurrent().ToString(), manager.currentIndex(), manager.currentIndex()));
						manager.mark();
						continue;
					}
					
					if (manager.getCurrent() == '}') {
						if (manager.headLength() > 0) tokens.Add(new TokenInfo(Token.INDENTIFIER, head.Substring(0, head.Length), manager.getLast(), manager.getCurrent() - 1));
						tokens.Add(new TokenInfo(Token.CLOSEBRACKET, manager.getCurrent().ToString(), manager.lastIndex(), manager.currentIndex()));
						manager.mark();
						continue;
					}

					if (manager.getCurrent() == '(') {
						if (manager.headLength() > 0) tokens.Add(new TokenInfo(Token.INDENTIFIER, head.Substring(0, head.Length), manager.getLast(), manager.getCurrent() - 1));
						tokens.Add(new TokenInfo(Token.OPENPARENTHESES, manager.getCurrent().ToString(), manager.lastIndex(), manager.currentIndex()));
						manager.mark();
						continue;
					}

					if (manager.getCurrent() == ')') {
						if (manager.headLength() > 0) tokens.Add(new TokenInfo(Token.INDENTIFIER, head.Substring(0, head.Length), manager.getLast(), manager.getCurrent() - 1));
						tokens.Add(new TokenInfo(Token.CLOSEPARENTHESES, manager.getCurrent().ToString(), manager.lastIndex(), manager.currentIndex()));
						manager.mark();
						continue;
					}

					if (!comment && manager.getCurrent() == '"') {
						if (label) {
							label = false;
							tokens.Add(new TokenInfo(Token.LABEL, head.Substring(1), manager.lastIndex(), manager.currentIndex()));
							manager.mark();
							continue;
						} else {
							label = true;
						}
					}
					
					if (manager.isCurrentWhitesapce() && !label) {
						if (manager.isLastWhitesapce()) {
							//tokens.Add(new TokenInfo(Token.UNKNOWN, head, manager.lastIndex(), manager.currentIndex()));
							manager.mark();
							continue;
						}
						
						decimal i = 0;
						bool isNumeric = decimal.TryParse(head, out i);

						if (isNumeric) {
							tokens.Add(new TokenInfo(Token.NUMBER, head, manager.lastIndex(), manager.currentIndex()));
							manager.mark();
							continue;
						}
						else {
							tokens.Add(new TokenInfo(Token.INDENTIFIER, head, manager.lastIndex(),
								manager.currentIndex()));
							manager.mark();
							continue;
						}
					}
				}
				
				manager.next();
			}

			return tokens;
		}
	}

	public enum Token {
		INDENTIFIER,
		LABEL,
		NUMBER,
		OPENBRACKET,
		CLOSEBRACKET,
		OPENPARENTHESES,
		CLOSEPARENTHESES,
		COMMENT,
		UNKNOWN
	}

	public class TokenInfo {
		private int startingIndex, endingIndex;
		private String value;
		private Token tokenType;

		public TokenInfo(Token tokenType, string value, int startingIndex, int endingIndex) {
			this.startingIndex = startingIndex;
			this.endingIndex = endingIndex;
			this.value = value;
			this.tokenType = tokenType;
		}

		public int StartingIndex => startingIndex;

		public int EndingIndex => endingIndex;

		public string Value => value;

		public Token TokenType => tokenType;

		public override string ToString() {
			return "[" + tokenType.ToString() + ": '" + value + "' (" + startingIndex + ", " + endingIndex +")]";
		}
	}
}