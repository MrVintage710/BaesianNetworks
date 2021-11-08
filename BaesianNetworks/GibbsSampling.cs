using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BaesianNetworks
{
	public class GibbsSampling
	{
		Random random = new Random();

		public double solve(string _query, BaesNetwork _network)
		{
			// The query split into given evidence and variables
			Tuple<string[], Evidence[]> data = SplitQuery(_query);

			// The initial evidence becomes the fixed evidence
			List<Evidence> fixedEvidence = new List<Evidence>();
			for (int i = 0; i < data.Item2.Length; i++)
				fixedEvidence.Add(data.Item2[i]);



			// Finding the Markov Blanket

			// The initial nodes passed in by the query
			BaesNode[] initialNodes = new BaesNode[data.Item1.Length];
            for (int i = 0; i < data.Item1.Length; i++)
				initialNodes[i] = _network.getNode(data.Item1[i]);

			// The parents of the starting nodes
			List<BaesNode> parents = new List<BaesNode>();
			// The children of the starting nodes
			List<BaesNode> children = new List<BaesNode>();
            for (int i = 0; i < initialNodes.Length; i++)
            {
				BaesNode[] tempParents = initialNodes[i].getParents();
                for (int x = 0; x < tempParents.Length; x++)
					parents.Add(tempParents[x]);

				BaesNode[] tempChildren = initialNodes[i].getChildren();
                for (int y = 0; y < tempChildren.Length; y++)
					children.Add(tempChildren[y]);
			}

			// The parents, of the children, of the starting nodes
			List<BaesNode> childrenParents = new List<BaesNode>();
            for (int i = 0; i < children.Count; i++)
            {
				BaesNode[] temp = children[i].getParents();
                for (int j = 0; j < temp.Length; j++)
					childrenParents.Add(temp[j]);
            }

			// List of all variables under the Markov Blanket
			List<Evidence> variables = new List<Evidence>();
			for (int i = 0; i < parents.Count; i++)
			{
				Evidence tempEvidence = new Evidence(parents[i].ToString(), "TRUE");
				variables.Add(tempEvidence);
			}
			for (int i = 0; i < children.Count; i++)
			{
				Evidence tempEvidence = new Evidence(children[i].ToString(), "TRUE");
				variables.Add(tempEvidence);
			}
			for (int i = 0; i < childrenParents.Count; i++)
			{
				Evidence tempEvidence = new Evidence(childrenParents[i].ToString(), "TRUE");
				variables.Add(tempEvidence);
			}

			// Creates the initial starting state
			List<string> initialState = new List<string>();
			// Randomly initializes the starting state
			for (int i = 0; i < variables.Count; i++)
            {
				int randomValue = random.Next(2);
				switch (randomValue)
				{
					case 0:
						initialState.Add("FALSE");
						break;
					case 1:
						initialState.Add("TRUE");
						break;
					default:
						break;
				}
			}
			for (int i = 0; i < fixedEvidence.Count; i++)
				initialState.Add(fixedEvidence[i].GetValue());

			// DEBUG:
			/*for (int i = 0; i < initialState.Count; i++)
            {
				Console.WriteLine(initialState[i]);
			}*/

			// Burn the first 'i' samples
			/*for (int i = 0; i < 10; i++)
				Sample(variables, fixedEvidence);*/

			for (int i = 0; i < initialState.Count; i++)
			{
				Console.WriteLine(initialState[i]);
			}
			Console.WriteLine("\n\n");
			List<string> burnState = Sample(10, variables, fixedEvidence, initialState);
			for (int i = 0; i < burnState.Count; i++)
			{
				Console.WriteLine(burnState[i]);
			}

			// Start collecting more relevant samples
			/*for (int i = 0; i < 10; i++)
				Sample(variables, fixedEvidence);*/

			return 1;
		}

		List<string> Sample(int _life, List<Evidence> _variables, List<Evidence> _fixedEvidence, List<string> _state)
        {
			if (_life > 0)
            {
				// Random value for selecting a random value
				int randomValue = random.Next(2);
				// Random value for selecting a random variable
				int randomIndex = random.Next(_variables.Count);

				// Randomly change the randomly selected variable
				switch (randomValue)
				{
					case 0:
						_state[randomIndex] = "FALSE";
						break;
					case 1:
						_state[randomIndex] = "TRUE";
						break;
					default:
						break;
				}

				Sample(_life - 1, _variables, _fixedEvidence, _state);
			}

			return _state;
        }

		Tuple<string[], Evidence[]> SplitQuery(string query)
		{
			var trimmed = String.Concat(query.Where(c => !Char.IsWhiteSpace(c)));
			var first = trimmed.Split('|');

			if (first.Length < 2)
				throw new InvalidDataException("'" + query + "' is not the right format for a query.");

			var queried = first[0].Split(',');
			var evidenceStrings = first[1].Split(',');

			var evidence = new List<Evidence>();
			foreach (var e in evidenceStrings)
			{
				var s = e.Split('=');

				if (s.Length < 2)
					throw new InvalidDataException("'" + e + "' is not the right format for evidence.");

				evidence.Add(new Evidence(s[0], s[1]));
			}

			return new Tuple<string[], Evidence[]>(queried, evidence.ToArray());
		}
	}
}