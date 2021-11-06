using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BaesianNetworks {
	public class VariableEliminationSolver : BayesianSolver {
		
		public double solve(string statement, BaesNetwork network) {
			var split = splitQuery(statement);
			verifyValues(split.Item1, split.Item2, network);
			return processQuery(statement, split.Item1, split.Item2, network);
		}

		private Tuple<string[], Evidence[]> splitQuery(string query) {
			var trimmed = String.Concat(query.Where(c => !Char.IsWhiteSpace(c)));
			var first = trimmed.Split('|');
			
			if(first.Length < 2) 
				throw new InvalidDataException("'" + query + "' is not the right format for a query.");

			var queried = first[0].Split(',');
			var evidenceStrings = first[1].Split(',');

			var evidence = new List<Evidence>();
			foreach (var e in evidenceStrings) {
				var s = e.Split('=');
				
				if(s.Length < 2) 
					throw new InvalidDataException("'" + e + "' is not the right format for evidence.");
				
				evidence.Add(new Evidence(s[0], s[1]));
			}
			
			return new Tuple<string[], Evidence[]>(queried, evidence.ToArray());
		}

		private void verifyValues(string[] variables, Evidence[] evidence, BaesNetwork network) {
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

		private double processQuery(string statement, string[] variables, Evidence[] evidence, BaesNetwork network) {
			IEnumerable<string> names = variables.Concat(evidence.Select(evidence1 => evidence1.GetName()));
			// Hidden Variables are all the variables not in the current query
			var hiddenVariables = network.getNodesExcept(names.ToArray());
			// Create an equation; Contents include: 
			// TODO: go variable by variable for the query
			// TODO: have no evidence, little evidence, moderate evidence : 3 Modes to solve
			// query each variable seperately
			foreach (string query in variables) {
				Equation equation = new Equation(statement, query, hiddenVariables, network, evidence);
			}
			//Console.WriteLine(string.Join(", ", hiddenVariables));
			
			return 0.0;
		}
	}
}