using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BaesianNetworks {
	public class QueryParser {

		/// <summary>
		/// This Method is a utility method that Parses Queries from simple text to something the program can work with.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="net"></param>
		/// <returns></returns>
		public static Tuple<string[], Evidence[]> parseQuery(string query, BaesNetwork net) {
			var result = splitQuery(query);
			verifyValues(result.Item1, result.Item2, net);
			return result;
		}

		/// <summary>
		/// This method Specifically parses the evidence in a query.
		/// </summary>
		/// <param name="evidence"></param>
		/// <returns></returns>
		/// <exception cref="InvalidDataException"></exception>
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

		/// <summary>
		/// Turn a list of evidence back into human readable text.
		/// </summary>
		/// <param name="evidence"></param>
		/// <returns></returns>
		public static string toEvidenceString(params Evidence[] evidence) {
			string result = "";
			foreach (var e in evidence) {
				result += e.GetName() + "=\"" + e.GetValue() + "\",";
			}

			return result;
		}
		
		/// <summary>
		/// This is a private method that breaks apart the query into its parts.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		private static Tuple<string[], Evidence[]> splitQuery(string query) {
			var trimmed = String.Concat(query.Where(c => !Char.IsWhiteSpace(c)));
			var first = trimmed.Split('|');
			
			if(first.Length < 2) 
				return new Tuple<string[], Evidence[]>(query.Split(','), new Evidence[]{});

			var queried = first[0].Split(',');
			var evidence = first.Length > 1 ? parseEvidence(first[1]) : new Evidence[] { };
			
			return new Tuple<string[], Evidence[]>(queried, evidence);
		}

		/// <summary>
		/// Takes the list of variables Queried and Evidence and checks to make sure they are valid
		/// </summary>
		/// <param name="variables"></param>
		/// <param name="evidence"></param>
		/// <param name="network"></param>
		/// <exception cref="InvalidDataException"></exception>
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