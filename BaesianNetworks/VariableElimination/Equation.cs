using System;
using System.Collections.Generic;
using System.Linq;
using BaesianNetworks.BIF;

namespace BaesianNetworks {
    public class Equation {
        private List<Component> equation;
        private string queryStatement;
        private BaesNode queryNode;
        

        public Equation(string statement, string query, IEnumerable<BaesNode> hiddenVars, BaesNetwork network, Evidence[] evidences) {
            equation = new List<Component>();
            queryStatement = statement;
            queryNode = network.getNode(query);
            
            // write append HiddenVars and SubQueries to the equation
            // going to have a summation for every hidden variable
            foreach (BaesNode bn in hiddenVars) {
                //summations.Add(new Summation(bn));
                equation.Add(new Summation(bn));
            }
            // DEBUG
            // going to have a subQuery for every node above the quiried node
            foreach (BaesNode bn in network.GetNodeMap().Values) {
                //subQueries.Add(new SubQuery(bn));
                // consider summations first
                
                // add to equation in bn does not match summation
                equation.Add(new SubQuery(bn));
            }
            // Order the Equation. Traveling through the equation right to left, compare each subquery to the closest
            // summation; if there exists a variable in the subquery which matches the summation, add that variable
            // to the summation (removing the subquery from the equation and adding it to the subquery).
            // If the subquery does not match any of the summations, then move it to the beginning of the equation
            //equation.Reverse();
        
            // DEBUG
            Console.WriteLine(this);
        }

        /// <summary>
        /// Orders the equation based on dependencies and depth
        /// </summary>
        private void OrderEquation() {
             
        }

        public override string ToString() {
            string o = "";
            o += "(" + queryStatement + ") = ";
            foreach (Component c in equation) {
                o += c.ToString();
            }
            return o;
        }
    }

    interface Component {
        String ComponentType();
        //Factor Solve();
    }

    class Summation : Component {
        private string sumOut;
        public string GetSumOut => sumOut;
        private List<SubQuery> process;
        public List<SubQuery> GetProcess => process;

        public String ComponentType() {
            return "summation";
        }

    public Summation(BaesNode bn) {
            sumOut = bn.getVariableName();
            process = new List<SubQuery>();
        }

        public void AddSubQuery(SubQuery sq) {
            // sub query is made out of a node
            process.Add(sq);
        }

        // public Factor Solve(Factor f) {
        //     return new Factor();
        // }

        public override string ToString() {
            string o = "";
            o += '\u2211' + "\"" + sumOut + "\"";
            foreach (SubQuery sq in process) {
                o += sq.ToString();
            }
            return o;
        }
    }

    class SubQuery : Component {
        private BaesNode variable;
        public BaesNode GetVariable => variable;
        private BaesNode[] dependencies;
        public BaesNode[] GetDependencies => dependencies;

        public String ComponentType() {
            return "subquery";
        }
        public SubQuery(BaesNode node) {
            variable = node;
            dependencies = node.getParents();
        }
        
        // public Factor Solve() {
        //     return new Factor();
        // }

        public override string ToString() {
            string o = "";
            o += " P(" + variable.getVariableName();
            if (dependencies.Length > 0) o += "|";
            foreach (BaesNode bn in dependencies) {
                o += bn.getVariableName();
                o += ",";
            }
            if (o.EndsWith(",")) o = o.Remove(o.Length-1);
            o += ")";
            return o;
        }
    }
    
}