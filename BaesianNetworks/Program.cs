using System;

namespace BaesianNetworks
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			//var solver = new VariableEliminationSolver();
			//var net = new BaesNetwork("test.bif");
			//solver.solve("BURGLARY|JOHNCALLS=TRUE,MARYCALLS=TRUE", net);

			GibbsSampling gsSolver = new GibbsSampling();
			BaesNetwork network = new BaesNetwork("test.bif");
			gsSolver.solve("BURGLARY|JOHNCALLS=TRUE,MARRYCALLS=TRUE", network);
		}
	}
}