using System;
using System.Collections.Generic;
using System.Linq;

namespace BaesianNetworks {
	/// <summary>
	/// This represents a probability table iside of the bayesian network
	/// </summary>
	public class BaesNode {
		private string name;
		private string[] values; 
		List<BaesNode> children = new List<BaesNode>();
		List<BaesNode> parents = new List<BaesNode>();
		List<TableEntry> tableEntries = new List<TableEntry>();

		public BaesNode(string name, params string[] values) {
			this.values = values;
			this.name = name;
		}

		/// <summary>
		/// Adds a child to the node.
		/// </summary>
		/// <param name="nodes"></param>
		public void addChildren(params BaesNode[] nodes) {
			foreach (var node in nodes) {
				if(!children.Contains(node)) children.AddRange(nodes);
			}
		}

		/// <summary>
		/// Adds a parrent to the given node
		/// </summary>
		/// <param name="nodes"></param>
		public void addParents(params BaesNode[] nodes) {
			foreach (var node in nodes) {
				if(!parents.Contains(node)) parents.AddRange(nodes);
				node.addChildren(this);
			}
			
			tableEntries.Clear();
		}

		/// <summary>
		/// Adds an entry in the table. The number of values should be equal to the number of parents.
		/// </summary>
		/// <param name="values"></param>
		/// <param name="probability"></param>
		public void addEntry(int[] values, double[] probability) {
			tableEntries.Add(new TableEntry(values, probability));
		}

		/// <summary>
		/// Returns the children of the Node
		/// </summary>
		/// <returns></returns>
		public BaesNode[] getChildren() {
			return children.ToArray();
		}
		
		/// <summary>
		/// returns the parents of the node.
		/// </summary>
		/// <returns></returns>
		public BaesNode[] getParents() {
			return parents.ToArray();
		}

		/// <summary>
		/// Returns the names of the parrents.
		/// </summary>
		/// <returns></returns>
		public string[] getParentNames() {
			return parents.Select(p => p.getVariableName()).ToArray();
		}

		/// <summary>
		/// This method and its overrides reference the probability tables and returns the probability given evidence
		/// </summary>
		/// <param name="parentValues"></param>
		/// <returns></returns>
		public double[] getProbabilities(params int[] parentValues) {
			if (parentValues.Length != getDepth()) return new double[]{};
			foreach (var entry in tableEntries) {
				if (entry.parentValuesEqual(parentValues)) return entry.ProbableValues;
			}

			return null;
		}

		public double[] getProbabilities(string evidence) {
			return getProbabilities(QueryParser.parseEvidence(evidence));
		}
		
		public double[] getProbabilities(params Evidence[] evidence) {
			var parentValues = new List<int>();
			foreach (var parent in parents) {
				foreach (var e in evidence) {
					if (parent.getVariableName() == e.GetName()) {
						parentValues.Add(parent.getIndex(e.GetValue()));
					}
				}
			}

			if (parentValues.Count != this.getDepth()) return null;
			return getProbabilities(parentValues.ToArray());
		}

		/// <summary>
		/// Gets the value at a given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public string getValue(int index) {
			return values[index];
		}

		/// <summary>
		/// Gives the index of a variable given the variable name.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int getIndex(string value) {
			for (var i = 0; i < values.Length; i++) {
				if (values[i].ToLower() == value.ToLower()) return i;
			}

			return -1;
		}
		
		public bool containsValue(string value) {
			foreach (var v in values) {
				if (v.ToLower() == value.ToLower()) return true;
			}

			return false;
		}

		public string[] GetValues() {
			return values;
		}

		public int numberOfValues() {
			return values.Length;
		}

		public int getDepth() {
			return parents.Count;
		}

		public string getVariableName() {
			return name;
		}

		public override bool Equals(object obj) {
			if (obj is BaesNode) return ((BaesNode) obj).name == this.name;
			return base.Equals(obj);
		}

		public override string ToString() {
			return this.name;
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