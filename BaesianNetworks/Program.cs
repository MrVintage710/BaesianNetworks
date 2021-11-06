using System;
using BaesianNetworks.BIF;

namespace BaesianNetworks {
	internal class Program {
		public static void Main(string[] args)
		{
			
			Console.WriteLine(new FactorEntry(new []{1, 2}).Equals(new FactorEntry(new []{1, 2})));
			BaesNetwork net = new BaesNetwork("test.bif");
			
			Console.WriteLine(string.Join(", ", net.getNodesExcept("Alarm")));
			
			var solver = new VariableEliminationSolver();
			solver.solve("ALARM|EARTHQUAKE=TRUE,MARYCALLS=TRUE", net);
			
			Matrix testMatrix = new Matrix(3, 3);
			Console.WriteLine(testMatrix.ToString());
		}
	}
}