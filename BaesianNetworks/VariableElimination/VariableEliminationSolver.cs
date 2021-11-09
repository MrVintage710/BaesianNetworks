using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BaesianNetworks {
	public class VariableEliminationSolver : BayesianSolver {
		
		public Report solve(string statement, BaesNetwork network) {
			var split = QueryParser.parseQuery(statement, network);
			return processQuery(statement, split.Item1, split.Item2, network);
		}

		private Report processQuery(string statement, string[] variables, Evidence[] evidence, BaesNetwork network) {
			IEnumerable<string> names = variables.Concat(evidence.Select(evidence1 => evidence1.GetName()));
			// Hidden Variables are all the variables not in the current query
			var hiddenVariables = network.getNodesExcept(names.ToArray());
			
			var allResults = new Report();
			
			foreach (string query in variables) {
				double[] unNormalizedResult = new double[network.getNode(query).GetValues().Length];
				Equation equation = new Equation(statement, query, hiddenVariables, network);
				int index = 0;
				foreach (var value in network.getNode(query).GetValues()) {
					var newEvidence = new List<Evidence>();
					newEvidence.Add(new Evidence(query, value));
					newEvidence.AddRange(evidence);
					var mul = new List<double>();
					foreach (var component in equation.getEquation()) {
						mul.Add(component.getFactor().solve(newEvidence.ToArray()));
					}
					Console.WriteLine(index);
					unNormalizedResult[index] = mul.Aggregate((l, n) => l * n);
					index++;
				}

				var normalizer = unNormalizedResult.Aggregate((l, n) => l + n);
				var results = unNormalizedResult.Select(i => i / normalizer);
				Console.WriteLine(string.Join(", ", results));
				
				allResults.addValue(query, results.ToArray());
			}
			
			return allResults;
		}
	}
}