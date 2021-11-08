using System;

namespace BaesianNetworks {
	internal class Program {
		public static void Main(string[] args) {
			var solver = new VariableEliminationSolver();
			var net = new BaesNetwork("test.bif");
			solver.solve("BURGLARY|JOHNCALLS=TRUE,MARYCALLS=TRUE", net);
		}
	}
}