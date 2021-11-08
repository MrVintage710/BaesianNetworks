using System;
using System.Collections.Generic;

namespace BaesianNetworks {
    public class FactorTree {
        private BaesNetwork network;

        private List<FactorNode> tree;
        //private Queue<FactorNode> parents;
        
        public FactorTree(List<BaesNode> factors, BaesNetwork network) {
            this.network = network;
            //parents = new Queue<FactorNode>();
            //parents = new Que { };
            //tree = new List<FactorNode>();
            FactorNode master_node = new FactorNode("master");
            ConstructPermutations(factors, master_node, 0 );
        }

        /// Construct the tree by recursively creating FactorNodes which link every permutation of a set of factors 
        private void ConstructPermutations(List<BaesNode> factors, FactorNode parents, int count)  {
            Console.WriteLine(count);
            while (count < factors.Count) { // TODO might change to Count + 1 here
                string considering = factors[count].getVariableName();
                // There is a considered node in the baes network, and I am looping through every possible 
                // variation of that variable
                foreach (string value in network.getNode(considering).GetValues()) {
                    FactorNode child = new FactorNode(considering + "=" + value);
                    parents.AddChild(child);
                    ConstructPermutations(factors, child, ++count);
                }
            }
        }
    }

    class FactorNode {
        
        private FactorNode parent;
        private List<FactorNode> Children;
        private string value;

        public FactorNode(string value = null) {
            Children = new List<FactorNode>();
            this.value = value;
        }

        public void SetParent(FactorNode parent) {
            this.parent = parent;
            parent.AddChild(this);
        }

        public void AddChild(FactorNode child) {
            Children.Add(child);
        }

    }
}