using System.Collections.Generic;

namespace BaesianNetworks {
	/// <summary>
	/// This Class was made to return the results from either of the algorithms. Holds the variables and their probable
	/// values.
	/// </summary>
	public class Report {
		Dictionary<string, double[]> map = new Dictionary<string, double[]>();

		public void addValue(string variableName, double[] values) {
			map[variableName] = values;
		}

		public override string ToString() {
			var s = "";

			foreach (var entry in map) {
				s += entry.Key + " | " + string.Join(", ", entry.Value) + "\n";
			}
			
			return s;
		}
	}
}