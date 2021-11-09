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
		public Dictionary<string, BaesNode> GetNodeMap() => nodeMap;
		private String networkName;
		
		private List<string> properties = new List<string>();
		
		public BaesNetwork(string filename) {
			string file = File.ReadAllText("./BIF/" + filename);
			var tokens = tokenize(file);
			parse(tokens);
		}

		/// <summary>
		/// This method Returns all of Nodes in the network except for the ones specified. 
		/// </summary>
		/// <param name="names"></param>
		/// <returns></returns>
		public IEnumerable<BaesNode> getNodesExcept(params string[] names) {
			return nodeMap.Values.Where((node) => {
				foreach (var n in names) {
					if (n.ToLower() == node.getVariableName().ToLower()) return false;
				}
				
				return true;
			});
		}

		/// <summary>
		/// This method Returns the node with the Variable name given.
		/// </summary>
		/// <param name="variableName"></param>
		/// <returns></returns>
		public BaesNode getNode(string variableName) {
			var key = variableName.ToLower();
			if (!nodeMap.ContainsKey(key)) return null;
			return nodeMap[key];
		}

		/// <summary>
		/// Checks to see if the variable is present in this graph.
		/// </summary>
		/// <param name="variableName"></param>
		/// <returns></returns>
		public bool hasVariable(string variableName) {
			return nodeMap.ContainsKey(variableName.ToLower());
		}

		/// <summary>
		/// Returns the name of the network.
		/// </summary>
		/// <returns></returns>
		public string getName() {
			return this.networkName;
		}
		
		/// <summary>
		/// This method represents the tokenization part of the parsing. Before the file is parsed proper,
		/// it is turned into tokens by this method. Later, the tokens are parsed into the data structure.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
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
					
					if (manager.getCurrent() == '|') {
						if (manager.headLength() > 0) processHead(head, tokens, manager, line, col);
						tokens.Enqueue(new TokenInfo(Token.BAR, manager.getCurrent().ToString(), line, col));
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

		/// <summary>
		/// This Method Parses the tokens into this data structure. Bellow are other methods that help in the parsing
		/// process, each being in charge of a different part of the program.
		/// </summary>
		/// <param name="tokens">
		private void parse(Queue<TokenInfo> tokens) {
			while (tokens.Count > 0) {
				var current = tokens.Dequeue();

				if (current.TokenType == Token.INDENTIFIER) {
					if(current.Value == "network") parseNetwork(tokens);
					if(current.Value == "variable") parseVariable(tokens);
					if(current.Value == "probability") parseProbability(tokens);
				}
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="head"></param>
		/// <param name="tokens"></param>
		/// <param name="manager"></param>
		/// <param name="line"></param>
		/// <param name="col"></param>
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

		private void parseProbability(Queue<TokenInfo> tokens) {
			checkToken(tokens.Dequeue(), Token.OPENPARENTHESES);
			var nodes = parseStatement(tokens);

			checkToken(tokens.Dequeue(), Token.OPENBRACKET);
			
			int expectedVariables = nodes.Item1.numberOfValues();
			foreach (var parent in nodes.Item2) {
				expectedVariables *= parent.numberOfValues();
			}

			int amountOfVariables = 0;
			while (true) {
				var current = tokens.Dequeue();
				if (current.TokenType == Token.INDENTIFIER && current.Value == "table") {
					var props = parseList(tokens);
					//Table logic
					amountOfVariables = props.Length;
					nodes.Item1.addEntry(new int[]{}, props.ToArray());
					break;
				}
				if (current.TokenType == Token.OPENPARENTHESES) {
					parseTableEntry(tokens, nodes.Item1, nodes.Item2);
				}
				if(current.TokenType == Token.CLOSEBRACKET) break;
			}
			
			//if(amountOfVariables != expectedVariables) throw new Exception("Not enough variables.");
		}

		private Tuple<BaesNode, BaesNode[]> parseStatement(Queue<TokenInfo> tokens) {
			var variable = tokens.Dequeue();
			checkToken(variable, Token.INDENTIFIER);
			checkNode(variable);
			BaesNode baseNode = nodeMap[variable.Value.ToLower()];
			
			var bar = tokens.Dequeue();
			if(bar.TokenType == Token.CLOSEPARENTHESES) 
				return new Tuple<BaesNode, BaesNode[]>(baseNode, new BaesNode[]{});
			checkToken(bar, Token.BAR);
			
			List<BaesNode> parents = new List<BaesNode>();
			
			while(true) {
				var parent = tokens.Dequeue();
				checkToken(parent, Token.INDENTIFIER);
				checkNode(parent);

				parents.Add(nodeMap[parent.Value.ToLower()]);
				
				var next = tokens.Dequeue();
				if(next.TokenType == Token.CLOSEPARENTHESES) break;
			}
			
			baseNode.addParents(parents.ToArray());
			return new Tuple<BaesNode, BaesNode[]>(baseNode, parents.ToArray());
		}

		private double[] parseList(Queue<TokenInfo> tokens) {
			List<double> numbers = new List<double>();
			while (true) {
				var current = tokens.Dequeue();
				if (current.TokenType == Token.NUMBER) {
					numbers.Add(Double.Parse(current.Value));
					var next = tokens.Dequeue();
					if(next.TokenType == Token.COMMA) continue;
					else if(next.TokenType == Token.ENDLINE) break;
					else throw new InvalidTokenException(next, Token.COMMA, Token.ENDLINE);
				}
				else throw new InvalidTokenException(current, Token.NUMBER);
			}

			return numbers.ToArray();
		}

		private void parseTableEntry(Queue<TokenInfo> tokens, BaesNode node, BaesNode[] parentNodes) {
			var parentValues = new List<int>();
			for (var i = 0; i < parentNodes.Length; i++) {
				var id = tokens.Dequeue();
				checkToken(id, Token.INDENTIFIER);
				int valueIndex = parentNodes[i].getIndex(id.Value);
				if (valueIndex < 0) throw new InvalidVariableException(id);
				parentValues.Add(valueIndex);
				
				var next = tokens.Dequeue();
				if(next.TokenType == Token.COMMA) continue;
				else if (next.TokenType == Token.CLOSEPARENTHESES) {
					if(i + 1 < parentNodes.Length) throw new NotEnoughVariablesException(next, i, parentNodes.Length);
				} else {
					throw new InvalidTokenException(next, Token.COMMA, Token.CLOSEPARENTHESES);
				}
			}

			var probabilities = new List<double>();
			for (var i = 0; i < node.numberOfValues(); i++) {
				var probability = tokens.Dequeue();
				checkToken(probability, Token.NUMBER);
				probabilities.Add(double.Parse(probability.Value));
				
				var next = tokens.Dequeue();
				if(next.TokenType == Token.COMMA) continue;
				else if (next.TokenType == Token.ENDLINE) {
					if(i + 1 < node.numberOfValues()) throw new NotEnoughVariablesException(next, i, parentNodes.Length);
				} else {
					throw new InvalidTokenException(next, Token.COMMA, Token.ENDLINE);
				}
			}
			
			node.addEntry(parentValues.ToArray(), probabilities.ToArray());
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
			
			var node = new BaesNode(variableName.Value, types.ToArray());
			nodeMap.Add(variableName.Value.ToLower(), node);
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
				
				checkToken(current, Token.INDENTIFIER);
				types.Add(current.Value);

				var next = tokens.Dequeue();
				if((i + 1 >= 2 && next.TokenType == Token.CLOSEBRACKET)) continue;
				if (next.TokenType != Token.COMMA) {
					throw new NotEnoughTypesException();
				}
			}

			checkToken(tokens.Dequeue(), Token.ENDLINE);
			
			return types.ToArray();
		}

		private void checkToken(TokenInfo tokenInfo, Token token) {
			if(tokenInfo.TokenType != token)
				throw new InvalidTokenException(tokenInfo, token);
		}

		private void checkNode(TokenInfo info) {
			if(!nodeMap.ContainsKey(info.Value.ToLower()))
				throw new InvalidVariableException(info);
		}
	}

	//This Enum represents all tokens that exist during parsing
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
		BAR,
		UNKNOWN
	}

	//This is a class that hold info about a token. This is used for the parser to report errors.
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

	/// <summary>
	/// These are the errors assiated with Parsing. These are thrown to let you know when you have messed up.
	/// </summary>
	class InvalidTokenException : Exception {

		public InvalidTokenException(TokenInfo info, params Token[] expected) 
			: base("(" + info.Line + ", " + info.Col + ") '" + string.Join(" or ", expected) + "' expected, found " + info.TokenType + ".") {}
		
	}

	class NotEnoughTypesException : Exception {
		public NotEnoughTypesException() : base("") { }
	}
	
	class TypeAlreadyDefinedException : Exception {
		public TypeAlreadyDefinedException(string propertyName) : base("A type is already defined for property '" + propertyName  + "'") { }
	}

	class InvalidVariableException : Exception {
		public InvalidVariableException(TokenInfo info) : 
			base("(" + info.Line + ", " + info.Col + ") Variable Does not exist in the current context: '" + info.Value +"'"){ }
	}

	class NotEnoughVariablesException : Exception {
		public NotEnoughVariablesException(TokenInfo info, int received, int expectedVariables) :
			base("(" + info.Line + ", " + info.Col + ") Not enough values. Expected: " + expectedVariables + "   Found: " + received) { }
	}
}