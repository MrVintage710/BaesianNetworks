using System;

namespace BaesianNetworks {
	internal class Program {
		public static void Main(string[] args) {
			var solver = new VariableEliminationSolver();
			var net = new BaesNetwork("test.bif");
			Console.WriteLine(string.Join(",", net.getNode("MARYCALLS").getProbabilities("ALARM=TRUE")));
			//solver.solve("BURGLARY|JOHNCALLS=TRUE,MARYCALLS=TRUE", net);
		}
	}
}