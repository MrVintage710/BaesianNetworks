using System;
using System.Collections.Generic;

namespace BaesianNetworks {
	public class BaesNode {
		private string[] values; 
		List<BaesNode> children = new List<BaesNode>();
		List<BaesNode> parents = new List<BaesNode>();
		List<int[]> tableEntries = new List<int[]>();

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

		public void addEntry(params int[] values) {
			int expectedSize = parents.Count + values.Length;
			if(values.Length != expectedSize) return;
			tableEntries.Add(values);
		}

		public BaesNode[] getChildren() {
			return children.ToArray();
		}
		
		public BaesNode[] getParent() {
			return parents.ToArray();
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
}