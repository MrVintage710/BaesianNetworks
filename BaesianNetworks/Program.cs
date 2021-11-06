using System;
using BaesianNetworks.BIF;

namespace BaesianNetworks {
	internal class Program {
		public static void Main(string[] args) {
			var solver = new VariableEliminationSolver();
			var net = new BaesNetwork("test.bif");
			solver.solve("BURGLARY|BURGLARY=TRUE", net);
		}
	}
}