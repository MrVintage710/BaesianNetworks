using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BaesianNetworks {
	public class VariableEliminationSolver : BayesianSolver {
		
		public double solve(string statement, BaesNetwork network) {
			var split = QueryParser.parseQuery(statement, network);
			return processQuery(statement, split.Item1, split.Item2, network);
		}

		private double processQuery(string statement, string[] variables, Evidence[] evidence, BaesNetwork network) {
			IEnumerable<string> names = variables.Concat(evidence.Select(evidence1 => evidence1.GetName()));
			// Hidden Variables are all the variables not in the current query
			var hiddenVariables = network.getNodesExcept(names.ToArray());

			foreach (var hv in hiddenVariables) {
				
			}
			
			
			foreach (string query in variables) {
				Equation equation = new Equation(statement, query, hiddenVariables, network);
				Queue<Component> queue = equation.AsQueue();
				ProcessEquation(queue);

				//pass in evidence out here

			}
			//Console.WriteLine(string.Join(", ", hiddenVariables));
			
			return 0.0;
		}

		private Double[] ProcessEquation(Queue<Component> queue) {
			Component toSolve = queue.Dequeue();
			if (queue.Count > 0) {
				double[] tempSolution = toSolve.Solve();
				queue.Peek().AddToTop(tempSolution);
				return ProcessEquation(queue);
			}
			return toSolve.Solve();
		}
	}
}