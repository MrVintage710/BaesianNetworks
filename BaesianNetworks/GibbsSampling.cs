using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BaesianNetworks
{
	/// <summary>
    /// Gibbs Sampling
    /// </summary>
	public class GibbsSampling
	{
		// Random class for incorporating randomness
		Random random = new Random();
		// List to hold the original query data
		List<string> query = new List<string>();
		// Variable to determine if a burn is in progress
		bool isBurning;

		public double solve(string _query, BaesNetwork _network)
		{
			// The query split into given evidence and variables
			Tuple<string[], Evidence[]> data = SplitQuery(_query);

			// Keeps track of original query
			for (int i = 0; i < data.Item1.Length; i++)
				query.Add(data.Item1[i]);

			// The initial evidence becomes the fixed evidence
			List<Evidence> fixedEvidence = new List<Evidence>();
			for (int i = 0; i < data.Item2.Length; i++)
				fixedEvidence.Add(data.Item2[i]);



			// FINDING MARKOV BLANKET

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

			// List of all variables (Nodes) under the Markov Blanket
			List<BaesNode> nodeVariables = new List<BaesNode>();
			for (int i = 0; i < parents.Count; i++)
				nodeVariables.Add(parents[i]);
			for (int i = 0; i < children.Count; i++)
				nodeVariables.Add(children[i]);
			for (int i = 0; i < childrenParents.Count; i++)
				nodeVariables.Add(childrenParents[i]);

			// List to hold all possilbe values that each node could be
			List<string[]> possibleValues = new List<string[]>();
			for (int i = 0; i < nodeVariables.Count; i++)
            {
				string[] tempPossibleValues = nodeVariables[i].GetValues();
				possibleValues.Add(tempPossibleValues);
            }



			// INITIAL STATE

			// Creates the initial starting state
			List<string> initialState = new List<string>();
			// Randomly initializes the starting state
			for (int i = 0; i < nodeVariables.Count; i++)
            {
				int randomIndex = random.Next(possibleValues[i].Length);
				initialState.Add(possibleValues[i][randomIndex]);
			}
			for (int i = 0; i < fixedEvidence.Count; i++)
				initialState.Add(fixedEvidence[i].GetValue());



			// SAMPLING


			//
			for (int i = 0; i < initialState.Count; i++)
			{
				Console.WriteLine(initialState[i]);
			}
			Console.WriteLine("\n\n");
			//

			List<int[]> valueCount = new List<int[]>();
			for (int i = 0; i < possibleValues.Count; i++)
            {
				int[] tempValueCount = new int[possibleValues[i].Length];
				valueCount.Add(tempValueCount);
			}

			// Burn the first 'i' samples
			isBurning = true;
			List<string> burnState = Sample(5000, nodeVariables, fixedEvidence, possibleValues, valueCount, initialState);
			for (int i = 0; i < burnState.Count; i++)
				Console.WriteLine(burnState[i]);
			isBurning = false;

			//
			Console.WriteLine("\n\n");
			//


			// Start collecting more relevant samples
			List<string> finalState = Sample(5000, nodeVariables, fixedEvidence, possibleValues, valueCount, burnState);
			for (int i = 0; i < finalState.Count; i++)
				Console.WriteLine(finalState[i]);

			// Creates a list of data that needs to be normalized
			List<int> dataToNormalize = new List<int>();
			for (int i = 0; i < nodeVariables.Count; i++)
			{
				if (query.Contains(nodeVariables[i].getVariableName()))
				{
					for (int j = 0; j < possibleValues[i].Length; j++)
						dataToNormalize.Add(valueCount[i][j]);
				}
			}

			for (int i = 0; i < valueCount.Count; i++)
				for (int j = 0; j < valueCount[i].Length; j++)
					Console.WriteLine("ValueCount " + i + " " + valueCount[i][j]);


			Console.WriteLine(Normalize(dataToNormalize)[0]);
			return Normalize(dataToNormalize)[0];
		}

		/// <summary>
        /// Normalizes input data so it all adds up to '1'
        /// </summary>
        /// <param name="_data"></param>
        /// <returns></returns>
		List<double> Normalize(List<int> _data)
        {
			int total = 0;
			for (int i = 0; i < _data.Count; i++)
				total += _data[i];

			List<double> probabilities = new List<double>();
			for (int i = 0; i < _data.Count; i++)
            {
				double tempDouble = (double)_data[i] / (double)total;
				probabilities.Add(tempDouble);
            }

			return probabilities;
        }

		/// <summary>
        /// Samples from list of variables, randomly setting its value and counting values of the queried variable
        /// </summary>
        /// <param name="_life"></param>
        /// <param name="_nodeVariables"></param>
        /// <param name="_fixedEvidence"></param>
        /// <param name="_possibleValues"></param>
        /// <param name="_valueCount"></param>
        /// <param name="_state"></param>
        /// <returns></returns>
		List<string> Sample(int _life, List<BaesNode> _nodeVariables, List<Evidence> _fixedEvidence, List<string[]> _possibleValues, List<int[]> _valueCount, List<string> _state)
        {
			if (_life > 0)
            {
				// Random value for selecting a random variable
				int randomIndex = random.Next(_nodeVariables.Count);
				// Random value for selecting a random value
				int randomValue = random.Next(_possibleValues[randomIndex].Length);

				// Randomly change the randomly selected variable
				_state[randomIndex] = _possibleValues[randomIndex][randomValue];

				// Count query values
				if (!isBurning)
                {
					for (int i = 0; i < _nodeVariables.Count; i++)
					{
						if (query.Contains(_nodeVariables[i].getVariableName()))
						{
							for (int j = 0; j < _possibleValues[i].Length; j++)
							{
								if (_state[i].Equals(_possibleValues[i][j]))
									_valueCount[i][j]++;
							}
						}
					}
				}

				Sample(_life - 1, _nodeVariables, _fixedEvidence, _possibleValues, _valueCount, _state);
			}
			return _state;
        }

		/// <summary>
        /// Splits the input query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
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