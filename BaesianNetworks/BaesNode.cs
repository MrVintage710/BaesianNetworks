using System;
using System.Collections.Generic;

namespace BaesianNetworks {
	public class BaesNode {
		private string[] values; 
		List<BaesNode> children = new List<BaesNode>();
		List<BaesNode> parents = new List<BaesNode>();

		public BaesNode(params string[] values) {
			this.values = values;
		}

		public void addChildren(params BaesNode[] nodes) {
			children.AddRange(nodes);
		}
		
		public void addParents(params BaesNode[] nodes) {
			parents.AddRange(nodes);
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
	}
}