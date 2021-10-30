using System;
using System.Collections.Generic;
using System.Linq;

namespace BaesianNetworks {
	public class BaesNode {
		private string[] values; 
		List<BaesNode> children = new List<BaesNode>();
		List<BaesNode> parents = new List<BaesNode>();
		List<TableEntry> tableEntries = new List<TableEntry>();

		public BaesNode(params string[] values) {
			this.values = values;
		}

		public void addChildren(params BaesNode[] nodes) {
			children.AddRange(nodes);
		}
		
		public void addParents(params BaesNode[] nodes) {
			parents.AddRange(nodes);
			tableEntries.Clear();
		}

		public void addEntry(int[] values, double[] probability) {
			tableEntries.Add(new TableEntry(values, probability));
		}

		public BaesNode[] getChildren() {
			return children.ToArray();
		}
		
		public BaesNode[] getParents() {
			return parents.ToArray();
		}

		public double[] getProbabilities(params int[] parentValues) {
			foreach (var entry in tableEntries) {
				if (entry.parentValuesEqual(parentValues)) return entry.ProbableValues;
			}

			return null;
		}

		public string getValue(int index) {
			return values[index];
		}

		public int getIndex(string value) {
			for (var i = 0; i < values.Length; i++) {
				if (values[i] == value) return i;
			}

			return -1;
		}
		
		public bool containsValue(string value) {
			foreach (var v in values) {
				if (v == value) return true;
			}

			return false;
		}

		public int numberOfValues() {
			return values.Length;
		}
	}

	public class TableEntry {
		private int[] parentValues;
		private double[] probableValues;

		public TableEntry(int[] parentValues, double[] probableValues) {
			this.parentValues = parentValues;
			this.probableValues = probableValues;
		}
		
		public bool parentValuesEqual(params int[] values) {
			return Enumerable.SequenceEqual(values, parentValues);
		}

		public int[] ParentValues => parentValues;

		public double[] ProbableValues => probableValues;
	}
}