using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BaesianNetworks {
	public class QueryParser {

		public static Tuple<string[], Evidence[]> parseQuery(string query, BaesNetwork net) {
			var result = splitQuery(query);
			verifyValues(result.Item1, result.Item2, net);
			return result;
		}

		public static Evidence[] parseEvidence(string evidence) {
			var pattern = @"[a-zA-Z1-9]+|""[^""]+""";
			var match = Regex.Matches(evidence, pattern).Cast<Match>().Select(m => new string(m.Value.Where(c => c != '"').ToArray())).ToArray();

			if(match.Length % 2 != 0) throw new InvalidDataException("Every Variable must have a value.");
			
			var evidences = new List<Evidence>();
			for (var i = 0; i < match.Length; i += 2) {
				evidences.Add(new Evidence(match[i], match[i+1]));
			}

			return evidences.ToArray();
		}

		public static string toEvidenceString(params Evidence[] evidence) {
			string result = "";
			foreach (var e in evidence) {
				result += e.GetName() + "=\"" + e.GetValue() + "\",";
			}

			return result;
		}
		
		private static Tuple<string[], Evidence[]> splitQuery(string query) {
			var trimmed = String.Concat(query.Where(c => !Char.IsWhiteSpace(c)));
			var first = trimmed.Split('|');
			
			if(first.Length < 2) 
				throw new InvalidDataException("'" + query + "' is not the right format for a query.");

			var queried = first[0].Split(',');
			var evidence = first.Length > 1 ? parseEvidence(first[1]) : new Evidence[] { };
			
			return new Tuple<string[], Evidence[]>(queried, evidence);
		}

		private static void verifyValues(string[] variables, Evidence[] evidence, BaesNetwork network) {
			foreach (var i in variables) {
				if(!network.hasVariable(i))
					throw new InvalidDataException("'" + i + "' is not a variable in network '" + network.getName() + "'");
			}

			foreach (var i in evidence) {
				if(!network.hasVariable(i.GetName()))
					throw new InvalidDataException("'" + i.GetName() + "' is not a variable in network '" + network.getName() + "'");

				var node = network.getNode(i.GetName());
				if(!node.containsValue(i.GetValue()))
					throw new InvalidDataException("'" + i.GetValue() + "' is not in the domain of variable '" + i.GetName() + "' in network '" + network.getName() + "'" );
			}
		}
	}
}