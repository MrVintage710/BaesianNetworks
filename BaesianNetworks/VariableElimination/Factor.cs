using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BaesianNetworks {
	
	public interface Factor {
		double solve(params Evidence[] evidence);

		string[] reliesOn();
	}

	public class BaseFactor : Factor {
		private BaesNode node;

		public BaseFactor(BaesNode node) {
			this.node = node;
		}

		public double solve(params Evidence[] evidence) {
			var values = node.getProbabilities(evidence);
			if (values != null) {
				foreach (var e in evidence) {
					if (e.GetName().ToLower() == node.getVariableName().ToLower()) {
						Console.WriteLine(values[node.getIndex(e.GetValue())]);
						return values[node.getIndex(e.GetValue())];
					}
						
				}
			}

			throw new InvalidDataException(
			                               "Not enough evidence solve factor for Node '" 
			                             + node.getVariableName() 
			                             + "'. Evidence needed: " + string.Join(", ", reliesOn())
			                             + ". Evidence had: " + string.Join<Evidence>(", ", evidence));
		}

		public string[] reliesOn() {
			var r = new List<string>();
			r.Add(node.getVariableName());
			r.AddRange(node.getParentNames());
			return r.ToArray();
		}
	}

	public class FactorSumation : Factor {
		private BaesNode node;
		private Factor[] factors;
		private string[] reliedUpon = new string[]{};
		
		public FactorSumation(BaesNode node, params Factor[] factors) {
			this.factors = factors;
			this.node = node;
			
			var relyList = new List<string>();
			foreach (var factor in factors) {
				relyList.AddRange(factor.reliesOn());
			}

			reliedUpon = relyList.Distinct<string>().Where(s => s.ToLower() != node.getVariableName().ToLower()).ToArray();
		}
		
		public double solve(params Evidence[] evidence) {
			if(reliedUpon.Length > evidence.Length)
				throw new InvalidDataException(
				                               "Not enough evidence solve factor for Node '" 
				                             + node.getVariableName() 
				                             + "'. Evidence needed: " + string.Join(", ", reliesOn())
				                             + ". Evidence had: " + string.Join<Evidence>(", ", evidence));
			
			var add = new List<double>();
			foreach (var valueName in node.GetValues()) {
				List<Evidence> allEvidence = new List<Evidence>();
				allEvidence.Add(new Evidence(node.getVariableName(), valueName));
				allEvidence.AddRange(evidence);
				
				List<double> mult = new List<double>();
				foreach (var factor in factors) {
					mult.Add(factor.solve(allEvidence.ToArray()));
				}

				var product = mult.Aggregate((c, n) => c * n);
				add.Add(product);
			}

			var sum = add.Aggregate((c, n) => c + n);
			Console.WriteLine("Sumation Solved | " + sum);
			return sum;
		}

		public string[] reliesOn() {
			return reliedUpon;
		}
	}
}