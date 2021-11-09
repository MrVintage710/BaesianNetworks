using System;

namespace BaesianNetworks {
	public interface BayesianSolver {

		Report solve(string statement, BaesNetwork network);

	}
}