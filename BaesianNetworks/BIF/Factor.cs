using System;
using System.Collections.Generic;

namespace BaesianNetworks.BIF {
	public class Factor {

		private int depth;
		private Dictionary<FactorEntry, double> entries = new Dictionary<FactorEntry, double>();
		
		public Factor(BaesNode node) {
			this.depth = node.getParents().Length + 1;

			foreach (var parent1 in node.getParents()) {
				foreach (var VARIABLE in node.getParents()) {
					
				}
			}
		}
		public double getProbability(params int[] values) {
			if (values.Length != depth) return -1.0;
			return entries[new FactorEntry(values)];
		}

		public int getDepth() {
			return depth;
		}
	}

	public class FactorEntry {
		private int[] lookup;

		public FactorEntry(int[] lookup) {
			this.lookup = lookup;
		}

		public override bool Equals(object obj) {
			if (obj is FactorEntry) {
				var other = (FactorEntry) obj;

				if (other.getDepth() == getDepth()) {
					for (var i = 0; i < getDepth(); i++) {
						if (other.lookup[i] != lookup[i]) return false;
					}

					return true;
				}
			}
			return base.Equals(obj);
		}

		public int[] getLookup() {
			return lookup;
		}
		
		public int getDepth() {
			return lookup.Length;
		}
	}
}