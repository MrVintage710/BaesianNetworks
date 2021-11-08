using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace BaesianNetworks {
	internal class Program {
		public static void Main(string[] args) {
			var solver = new VariableEliminationSolver();
			var net = new BaesNetwork("test.bif");
			//Console.WriteLine(string.Join(",", net.getNode("MARYCALLS").getProbabilities("ALARM=TRUE")));
			//solver.solve("BURGLARY|JOHNCALLS=TRUE,MARYCALLS=TRUE", net);
			
			var pattern = @"[a-zA-Z1-9]+|""[^""]+""";
			var match = Regex.Matches("Regex=Test, Test2=\"The=test\"", pattern).Cast<Match>().Select(m => new string(m.Value.Where(c => c != '"').ToArray())).ToArray();
			Evidence[] evidence = QueryParser.parseEvidence("JOHNCALLS=TRUE,MARYCALLS=TRUE");
			Console.WriteLine(string.Join<Evidence>(", ", evidence));
		}
	}
}