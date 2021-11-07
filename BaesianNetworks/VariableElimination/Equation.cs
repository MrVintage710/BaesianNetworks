using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace BaesianNetworks {
    public class Equation {
        private List<Component> equation;
        private string queryStatement;
        private BaesNode queryNode;

        public Equation(string statement, string query, IEnumerable<BaesNode> hiddenVars, BaesNetwork network) {
            equation = new List<Component>();
            queryStatement = statement;
            queryNode = network.getNode(query);
            List<SubQuery> subQueries = new List<SubQuery>();
            List<Summation> summations = new List<Summation>();
            // write append HiddenVars and SubQueries to the equation
            // going to have a summation for every hidden variable
            foreach (BaesNode hbn in hiddenVars) {
                //summations.Add(new Summation(bn));
                summations.Add(new Summation(hbn));
            }
            // going to have a subQuery for every node above the quiried node
            foreach (BaesNode bn in network.GetNodeMap().Values) {
                //subQueries.Add(new SubQuery(bn));
                // add to equation in bn does not match summation
                subQueries.Add(new SubQuery(bn));
            }
            OrderEquation(summations, subQueries);
            EquationToFactors();
            // DEBUG
            Console.WriteLine(this);
        }

        public override string ToString() {
            string o = "";
            o += "(" + queryStatement + ") = ";
            foreach (Component c in equation) {
                o += c.ToString();
            }
            return o;
        }

        private void OrderEquation(List<Summation> summations, List<SubQuery> subQueries) {
            // Order the Equation. Traveling through the equation right to left, compare each subquery to the closest
            // summation; if there exists a variable in the subquery which matches the summation, add that variable
            // to the summation (removing the subquery from the equation and adding it to the subquery).
            // If the subquery does not match any of the summations, then move it to the beginning of the equation
            summations.Reverse();
            foreach (SubQuery sq in subQueries) {
                foreach (Summation summation in summations) {
                    if (sq.GetSignature.Contains(summation.GetSumOut) && !sq.marked) {
                        summation.AddSubQuery(sq);
                        sq.marked = true;
                    }
                }
            }

            foreach (Summation summation in summations) equation.Add(summation);
            foreach (SubQuery sq in subQueries) {
                if (!sq.marked) equation.Add(sq);
            }

            equation.Reverse();
        }

        private void EquationToFactors() {
        }

        public Queue<Component> AsQueue() {
            Queue<Component> equation_queue = new Queue<Component>();
            for (int i = equation.Count-1; i > -1; i--) {
                equation_queue.Enqueue(equation[i]);
            }
            return equation_queue;
        }
    }

    public interface Component {
        double[] Solve(params Evidence[] evidences);
        void AddToTop(double[] tempSolution);
    }

    
    class Summation : Component {
        private string sumOut;
        public string GetSumOut => sumOut;
        private List<SubQuery> process;
        private double[] solved;
        public List<SubQuery> GetProcess => process;

        public Summation(BaesNode bn) {
            sumOut = bn.getVariableName();
            process = new List<SubQuery>();
        }

        public void AddSubQuery(SubQuery sq) {
            // sub query is made out of a node
            process.Add(sq);
        }

        public void AddToTop(double[] tempSolution) {
            solved = tempSolution;
        }

        public double[] Solve(params Evidence[] evidences) {
            
        } 
        
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
        private string signature = "";
        public string GetSignature => signature;
        public bool marked = false;
        
        // Factor relevent info below
        private List<BaesNode> factors;
        private double[] solved;
        
        public SubQuery(BaesNode node) {
            factors = new List<BaesNode>();
            variable = node;
            dependencies = node.getParents();
            // define signature
            signature += variable.getVariableName() + " ";
            foreach (BaesNode bn in dependencies) { 
                signature += bn.getVariableName() + " ";
                factors.Add(bn);
            }
            factors.Add(variable);
        }

        public void AddToTop(double[] tempSolution) {
            solved = tempSolution;
        }

        public double[] Solve(params Evidence[] evidences) {
            
        }

        public override string ToString() {
            string o = "";
            o += " P(" + variable.getVariableName();
            if (dependencies.Length > 0) o += "|";
            foreach (BaesNode bn in dependencies) {
                o += bn.getVariableName();
                o += ",";
            }
            if (o.EndsWith(",")) o = o.Remove(o.Length-1);
            o += ") ";
            return o;
        }
    }
    
}