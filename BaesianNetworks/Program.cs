using System;

namespace BaesianNetworks {
	internal class Program {
		public static void Main(string[] args)
		{
			//BaesNetwork net = new BaesNetwork("test.bif");
			Matrix testMatrix = new Matrix(3, 3);
			Console.WriteLine(testMatrix.ToString());
		}
	}
}