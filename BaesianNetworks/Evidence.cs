namespace BaesianNetworks
{
    public class Evidence
    {
        string name;
        string value;

        public Evidence(string _variableName, string _variableValue)
        {
            name = _variableName;
            value = _variableValue;
        }

        public string GetName()
        {
            return name;
        }

        public string GetValue()
        {
            return value;
        }
    }
}
