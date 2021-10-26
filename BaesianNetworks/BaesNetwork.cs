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
		private String networkName;
		
		private List<string> properties = new List<string>();
		
		public BaesNetwork(string filename) {
			string file = File.ReadAllText("./BIF/" + filename);
			var tokens = tokenize(file);
			Console.WriteLine(string.Join(",\n", tokens));
			parse(tokens);
			Console.WriteLine(networkName);
		}
		
		private Queue<TokenInfo> tokenize(string file) {
			ParceManager manager = new ParceManager(file);
			Queue<TokenInfo> tokens = new Queue<TokenInfo>();

			bool comment = false;
			bool label = false;

			int line = 1;
			int col = 1;

			while (manager.hasNext()) {
				var head = manager.getHead();
				col++;
				
				if (head == "//") comment = true;

				if (manager.getCurrent() == '\n') {
					line++;
					col = 1;
				}
				
				if (comment && manager.getCurrent() == '\n') {
					tokens.Enqueue(new TokenInfo(Token.COMMENT, head, line, col));
					comment = false;
					manager.mark();
					continue;
				}

				if (!comment) {
					if (!label && manager.getCurrent() == '{') {
						if (manager.headLength() > 0) processHead(head.Substring(0, head.Length), tokens, manager, line, col);
						tokens.Enqueue(new TokenInfo(Token.OPENBRACKET, manager.getCurrent().ToString(), line, col));
						manager.mark();
						continue;
					}
					
					if (!label && manager.getCurrent() == '}') {
						if (manager.headLength() > 0) processHead(head.Substring(0, head.Length), tokens, manager, line, col);
						tokens.Enqueue(new TokenInfo(Token.CLOSEBRACKET, manager.getCurrent().ToString(), line, col));
						manager.mark();
						continue;
					}

					if (!label && manager.getCurrent() == '(') {
						if (manager.headLength() > 0) processHead(head.Substring(0, head.Length), tokens, manager, line, col);
						tokens.Enqueue(new TokenInfo(Token.OPENPARENTHESES, manager.getCurrent().ToString(), line, col));
						manager.mark();
						continue;
					}

					if (!label && manager.getCurrent() == ')') {
						if (manager.headLength() > 0) processHead(head.Substring(0, head.Length), tokens, manager, line, col);
						tokens.Enqueue(new TokenInfo(Token.CLOSEPARENTHESES, manager.getCurrent().ToString(), line, col));
						manager.mark();
						continue;
					}
					
					if (!label && manager.getCurrent() == '[') {
						if (manager.headLength() > 0) processHead(head.Substring(0, head.Length), tokens, manager, line, col);
						tokens.Enqueue(new TokenInfo(Token.OPENSQUAREBRACKET, manager.getCurrent().ToString(), line, col));
						manager.mark();
						continue;
					}

					if (!label && manager.getCurrent() == ']') {
						if (manager.headLength() > 0) processHead(head.Substring(0, head.Length), tokens, manager, line, col);
						tokens.Enqueue(new TokenInfo(Token.CLOSESQUAREBRACKET, manager.getCurrent().ToString(), line, col));
						manager.mark();
						continue;
					}

					if (manager.getCurrent() == ';') {
						if (manager.headLength() > 0) processHead(head, tokens, manager, line, col);
						tokens.Enqueue(new TokenInfo(Token.ENDLINE, manager.getCurrent().ToString(), line, col));
						manager.mark();
						continue;
					}
					
					if (manager.getCurrent() == ',') {
						if (manager.headLength() > 0) processHead(head, tokens, manager, line, col);
						tokens.Enqueue(new TokenInfo(Token.COMMA, manager.getCurrent().ToString(), line, col));
						manager.mark();
						continue;
					}

					if (manager.getCurrent() == '"') {
						if (label) {
							label = false;
							tokens.Enqueue(new TokenInfo(Token.LABEL, head.Substring(1), line, col));
							manager.mark();
							continue;
						} else {
							label = true;
						}
					}

					if (manager.isCurrentWhitesapce() && !label) {
						if (manager.isLastWhitesapce()) {
							//tokens.Enqueue(new TokenInfo(Token.UNKNOWN, head, line, col));
							manager.mark();
							continue;
						}
						
						decimal i = 0;
						bool isNumeric = decimal.TryParse(head, out i);

						if (isNumeric) {
							tokens.Enqueue(new TokenInfo(Token.NUMBER, head, line, col));
							manager.mark();
							continue;
						}
						else {
							tokens.Enqueue(new TokenInfo(Token.INDENTIFIER, head, line, col));
							manager.mark();
							continue;
						}
					}
				}
				
				manager.next();
			}

			return tokens;
		}

		private void processHead(string head, Queue<TokenInfo> tokens, ParceManager manager, int line, int col) {
			if (manager.isLastWhitesapce()) {
				//tokens.Enqueue(new TokenInfo(Token.UNKNOWN, head, line, col));
				return;
			}
						
			decimal i = 0;
			bool isNumeric = decimal.TryParse(head, out i);

			if (isNumeric) {
				tokens.Enqueue(new TokenInfo(Token.NUMBER, head, line, col));
			} else {
				tokens.Enqueue(new TokenInfo(Token.INDENTIFIER, head, line, col));
			}
		}

		private void parse(Queue<TokenInfo> tokens) {
			while (tokens.Count > 0) {
				var current = tokens.Dequeue();

				if (current.TokenType == Token.INDENTIFIER) {
					if(current.Value == "network") parseNetwork(tokens);
					if(current.Value == "variable") parseVariable(tokens);
				}
			}
		}

		private void parseVariable(Queue<TokenInfo> tokens) {
			var variableName = tokens.Dequeue();
			checkToken(variableName, Token.INDENTIFIER);
			
			checkToken(tokens.Dequeue(), Token.OPENBRACKET);

			var properties = new List<string>();
			var types = new List<string>();

			bool hasDefinedType = false;
			
			TokenInfo current;
			do {
				current = tokens.Dequeue();
				if (current.TokenType == Token.INDENTIFIER && current.Value == "property") properties.AddRange(parseProperty(tokens));
				if (current.TokenType == Token.INDENTIFIER && current.Value == "type" && hasDefinedType)
					throw new TypeAlreadyDefinedException(variableName.Value);
				if (current.TokenType == Token.INDENTIFIER && current.Value == "type" && !hasDefinedType) {
					types.AddRange(parseType(tokens));
					hasDefinedType = true;
				}
			} while (current.TokenType != Token.CLOSEBRACKET);
			
			var node = new BaesNode(types.ToArray());
			nodeMap.Add(variableName.Value, node);
		}

		private void parseNetwork(Queue<TokenInfo> tokens) {
			var networkName = tokens.Dequeue();
			checkToken(networkName, Token.INDENTIFIER);

			this.networkName = networkName.Value;

			checkToken(tokens.Dequeue(), Token.OPENBRACKET);

			TokenInfo current;
			do {
				current = tokens.Dequeue();
				if (current.TokenType == Token.INDENTIFIER && current.Value == "property") properties.AddRange(parseProperty(tokens));
			} while (current.TokenType != Token.CLOSEBRACKET);
		}

		private string[] parseProperty(Queue<TokenInfo> tokens) {
			List<string> comments = new List<string>();
			
			TokenInfo current;
			do {
				current = tokens.Dequeue();
				if(current.TokenType == Token.ENDLINE) break;
				if (current.TokenType == Token.LABEL) comments.Add(current.Value);
				else throw new InvalidTokenException(current, Token.LABEL);
			} while (current.TokenType != Token.ENDLINE);

			return comments.ToArray();
		}

		private string[] parseType(Queue<TokenInfo> tokens) {
			var typeDescription = tokens.Dequeue();
			checkToken(typeDescription, Token.INDENTIFIER);
			
			checkToken(tokens.Dequeue(), Token.OPENSQUAREBRACKET);

			var amountToken = tokens.Dequeue();
			checkToken(amountToken, Token.NUMBER);
			var amount = double.Parse(amountToken.Value);
			
			checkToken(tokens.Dequeue(), Token.CLOSESQUAREBRACKET);
			checkToken(tokens.Dequeue(), Token.OPENBRACKET);

			var types = new List<string>();
			
			for (var i = 0; i < amount; i++) {
				var current = tokens.Dequeue();
				if(current.TokenType == Token.CLOSEBRACKET)
					throw new NotEnoughTypesException(i, (int) amount);
				
				checkToken(current, Token.INDENTIFIER);
				types.Add(current.Value);
			}

			checkToken(tokens.Dequeue(), Token.CLOSEBRACKET);
			checkToken(tokens.Dequeue(), Token.ENDLINE);
			
			return types.ToArray();
		}

		private void checkToken(TokenInfo tokenInfo, Token token) {
			if(tokenInfo.TokenType != token)
				throw new InvalidTokenException(tokenInfo, token);
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
		OPENSQUAREBRACKET,
		CLOSESQUAREBRACKET,
		COMMENT,
		ENDLINE,
		COMMA,
		UNKNOWN
	}

	public class TokenInfo {
		private int line, col;
		private String value;
		private Token tokenType;

		public TokenInfo(Token tokenType, string value, int line, int col) {
			this.line = line;
			this.col = col;
			this.value = value;
			this.tokenType = tokenType;
		}

		public int Line => line;

		public int Col => col;

		public string Value => value;

		public Token TokenType => tokenType;

		public override string ToString() {
			return "[" + tokenType + ": '" + value + "' (" + line + ", " + col +")]";
		}
	}

	public class InvalidTokenException : Exception {

		public InvalidTokenException(TokenInfo info, Token expected) : base("(" + info.Line + ", " + info.Col + ") '" + expected + "' expected, found " + info.TokenType + ".") {}
		
	}

	public class NotEnoughTypesException : Exception {
		public NotEnoughTypesException(int found, int required) : base("") { }
	}
	
	public class TypeAlreadyDefinedException : Exception {
		public TypeAlreadyDefinedException(string propertyName) : base("A type is already defined for property '" + propertyName  + "'") { }
	}
}