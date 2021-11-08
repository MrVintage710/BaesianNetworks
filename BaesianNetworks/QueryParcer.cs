using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BaesianNetworks {
	public class QueryParser {

		public static Tuple<string[], Evidence[]> parseQuery(string query, BaesNetwork net) {
			var result = splitQuery(query);
			verifyValues(result.Item1, result.Item2, net);
			return result;
		}

		public static Evidence[] parseEvidence(string evidence) {
			var evidenceStrings = evidence.Split(',');

			var evidences = new List<Evidence>();
			foreach (var e in evidenceStrings) {
				var s = e.Split('=');
				
				if(s.Length < 2) 
					throw new InvalidDataException("'" + e + "' is not the right format for evidence.");
				
				evidences.Add(new Evidence(s[0], s[1]));
			}

			return evidences.ToArray();
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