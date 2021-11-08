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

        public Stack<Component> root() {
            var stack = new Stack<Component>();
            Component current = null;
            foreach (var comp in equation) {
                if (comp is SubQuery) {
                    stack.Push(comp);
                    continue;
                }

                if (current != null) {
                    current.addComponent(comp);
                    current = comp;
                }
                else {
                    current = comp;
                    stack.Push(current);
                }
            }
            
            return stack;
        }
    }

    public interface Component {
        double[] Solve(BaesNetwork met, params Evidence[] evidences);
        void AddToTop(double[] tempSolution);

        void addComponent(Component component);
    }

    
    class Summation : Component {
        private string sumOut;
        public string GetSumOut => sumOut;
        private List<Component> process;
        private double[] solved;
        public List<Component> GetProcess => process;

        public Summation(BaesNode bn) {
            sumOut = bn.getVariableName();
            process = new List<Component>();
        }

        public void AddSubQuery(SubQuery sq) {
            // sub query is made out of a node
            process.Add(sq);
        }

        public void AddToTop(double[] tempSolution) {
            solved = tempSolution;
        }

        public void addComponent(Component component) {
            process.Add(component);
        }

        public double[] Solve(BaesNetwork net, params Evidence[] evidences) {
            var results = new Queue<double[]>();
            foreach (var value in net.getNode(sumOut).GetValues()) {
                var inner_result = new Queue<double[]>();
                process.Reverse();
                foreach (var p in process) {
                    var evidence = new Evidence(sumOut, value);
                    inner_result.Enqueue(p.Solve(net, evidences.Concat(new[] {evidence}).ToArray()));
                }
                results.Enqueue(factorProduct(inner_result));
            }
            return sumFacotrs(results);
        } 
        
        public override string ToString() {
            string o = "";
            o += '\u2211' + "\"" + sumOut + "\"";
            foreach (Component sq in process) {
                o += sq.ToString();
            }
            return o;
        }

        private double[] factorProduct(Queue<double[]> factors, double[] current = null) {
            if (factors.Count == 0) return current;
            
            var next = factors.Dequeue();
            if (current == null) {
                return factorProduct(factors, current = next);
            }

            if (current.Length == 1 && next.Length == 1) {
                return factorProduct(factors, new[] {current[0] * next[0]});
            }

            if (current.Length == 1 && next.Length > 1) {
                var values = new List<double>();
                for (var i = 0; i < next.Length; i++) {
                    values.Add(next[i] * current[0]);
                }
                return factorProduct(factors, values.ToArray());
            }

            if (current.Length > 1 && next.Length == 1) {
                var values = new List<double>();
                for (var i = 0; i < current.Length; i++) {
                    values.Add(current[i] * next[0]);
                }
                return factorProduct(factors, values.ToArray());
            }

            if (current.Length > 1 && next.Length > 1) {
                if (current.Length != next.Length) throw new Exception("Product cannot be done.");
                var values = new List<double>();
                for (var i = 0; i < current.Length; i++) {
                    values.Add(current[i] * next[i]);
                }
                return factorProduct(factors, values.ToArray());
            }

            return new double[] { };
        }

        private double[] sumFacotrs(Queue<double[]> factors, double[] sum = null) {
            if (factors.Count == 0) return sum;
            var next = factors.Dequeue();
            if (sum == null) return sumFacotrs(factors, sum = next);

            if (sum.Length == next.Length) {
                var values = new List<double>();
                for (var i = 0; i < sum.Length; i++) {
                    values.Add(sum[i] + next[i]);
                }

                return sumFacotrs(factors, values.ToArray());
            }

            throw new Exception("Cannot add factors.");
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

        public void addComponent(Component component) { }

        public double[] Solve(BaesNetwork net, params Evidence[] evidences) {
            List<int> e = new List<int>();
            foreach (var evidenceNeeded in variable.evidenceNeeded()) {
                bool foundMatch = false;
                foreach (var evidence in evidences) {
                    if (evidence.GetName().ToLower() == evidenceNeeded.ToLower()) {
                        e.Add(net.getNode(evidenceNeeded).getIndex(evidence.GetValue()));
                        foundMatch = true;
                        break;
                    }
                }

                if (!foundMatch) {
                    
                }
            }
            
            
            return variable.getProbabilities(e.ToArray());
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