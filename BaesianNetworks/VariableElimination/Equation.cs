using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace BaesianNetworks {
    public class Equation {
        private List<Component> equation;
        private string queryStatement;
        private BaesNode queryNode;
        private List<Factor> factorList; 


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

            var index = 0;
            Component current = equation[index];
            while (index < equation.Count - 1) {
                Component next = equation[index+1];

                if (current is SubQuery) {
                    index++;
                    current = equation[index];
                    continue;
                }

                if (current is Summation) {
                    var sum = (Summation) current;
                    sum.AddComponent(next);
                    current = next;
                    equation.RemoveAt(index + 1);
                }
            }

            // DEBUG
            Console.WriteLine(this);
        }

        public List<Component> getEquation() {
            return equation;
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
                        summation.AddComponent(sq);
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
    }

    public interface Component {
        Factor getFactor();
    }

    
    class Summation : Component {
        private BaesNode sumOut;
        public string GetSumOut => sumOut.getVariableName();
        private List<Component> process;

        public Summation(BaesNode bn) {
            sumOut = bn;
            process = new List<Component>();
        }

        public void AddComponent(Component sq) {
            process.Add(sq);
        }

        public override string ToString() {
            string o = "";
            o += '\u2211' + "\"" + sumOut + "\"";
            foreach (Component sq in process) {
                o += sq.ToString();
            }
            return o;
        }

        public Factor getFactor() {
            List<Factor> subFactors = new List<Factor>();
            foreach (var subQuery in process) {
                subFactors.Add(subQuery.getFactor());
            }
            return new FactorSumation(sumOut, subFactors.ToArray());
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
        
        // Table made from factors
        private List<string> factorComboTable;
        private List<double> factorProbTable;
        
        public SubQuery(BaesNode node) {
            factors = new List<BaesNode>();
            factorComboTable = new List<string>();
            factorProbTable = new List<double>();
            variable = node;
            dependencies = node.getParents();
            // define signature
            signature += variable.getVariableName() + " ";
            foreach (BaesNode bn in dependencies) { 
                signature += bn.getVariableName() + " ";
                factors.Add(bn);
            }
            factors.Add(variable);
            
            // Create factor table
            double sections = 2;
            double total = Math.Pow(2,factors.Count());
            
            // TODO check if this works , want order: greatest -> least 
            //factors.Sort((x,y)=>x.numberOfValues().CompareTo(y.numberOfValues()));
            factors.Sort((x,y)=>y.numberOfValues().CompareTo(x.numberOfValues()));
            
            int driver = factors[0].GetValues().Length;
            int element_count = 0;
            foreach (BaesNode factor in factors) {
                 
                for (int s = 0; s < sections; s++) {
                            
                }
                sections = Math.Pow(sections, 2);
            } 
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

        public Factor getFactor() {
            return new BaseFactor(variable);
        }
    }
    
}