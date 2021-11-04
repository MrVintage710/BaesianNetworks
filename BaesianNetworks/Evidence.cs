namespace BaesianNetworks
{
    public class Evidence
    {
        // The name associated with this evidence
        string name;

        // The value associated with this evidence
        string value;

        /// <summary>
        /// Constructs a new evidence object with a given name and value
        /// </summary>
        /// <param name="_variableName"></param>
        /// <param name="_variableValue"></param>
        public Evidence(string _variableName, string _variableValue)
        {
            name = _variableName;
            value = _variableValue;
        }

        /// <summary>
        /// Returns the name attached to a piece of evidence
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Returns the value attached to a piece of evidence
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            return value;
        }
    }
}