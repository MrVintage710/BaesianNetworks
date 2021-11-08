using System;
using System.Collections.Generic;

namespace BaesianNetworks {
    public class FactorTree {
        private BaesNetwork network;

        private List<FactorNode> tree;
        //private Queue<FactorNode> parents;
        
        public FactorTree(List<BaesNode> factors, BaesNetwork network) {
            this.network = network;
            FactorNode startingNode = new FactorNode("start");
            ConstructPermutations(factors, startingNode, 0);
        }

         // Construct the tree by recursively creating FactorNodes which link every permutation of a set of factors 
         private void ConstructPermutations(List<BaesNode> factors, FactorNode parent, int count)  {
              Console.WriteLine(count);
              while (count < factors.Count) { // TODO might change to Count + 1 here
                  string considering = factors[count].getVariableName();
                  // There is a considered node in the baes network, and I am looping through every possible 
                  // variation of that variable
                  foreach (string value in network.getNode(considering).GetValues()) {
                      FactorNode child = new FactorNode(considering + "=" + value);
                      parent.AddChild(child);
                      ConstructPermutations(factors, child, ++count);
                  }
                  //return ConstructPermutations(factors, parent, --count);
              }
         }
         
         
        
    }

    class FactorNode {
        
        private FactorNode parent;
        private List<FactorNode> Children;
        private string value ;
        // complexity is the total number of factors which makes up a probability combination

        public FactorNode(string value) {
            this.value = value;
            Children = new List<FactorNode>();
        }
        
        public void AddChild(FactorNode child) {
            Children.Add(child);
        }
         private void SetParent(FactorNode parent) { 
             this.parent = parent;
             parent.AddChild(this);
         }

    }
}