using System.Collections.Generic;

namespace BaesianNetworks {
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