﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BaesianNetworks {
	internal class Program {
		public static void Main(string[] args) {
			var solver = new VariableEliminationSolver();
			var net = new BaesNetwork("insurance.bif");

			var report = solver.solve("MedCost|Age=Adolescent,GoodStudent=False,SeniorTrain=False,DrivQuality=Poor", net);
			Console.WriteLine(report);
			
			//exampleVEProcess(net);
		}
		
		/// <summary>
		/// This function is not mandatory for your algorithm to work, this is just a walk through on how the VE works
		/// </summary>
		/// <param name="net"></param>
		private static void exampleVEProcess(BaesNetwork net) {
			BaseFactor f1 = new BaseFactor(net.getNode("BURGLARY"));
			BaseFactor f2 = new BaseFactor(net.getNode("EARTHQUAKE"));
			BaseFactor f3 = new BaseFactor(net.getNode("ALARM"));
			BaseFactor f4 = new BaseFactor(net.getNode("MARYCALLS"));
			BaseFactor f5 = new BaseFactor(net.getNode("JOHNCALLS"));
			
			FactorSumation f6 = new FactorSumation(net.getNode("EARTHQUAKE"), f2, f3);
			FactorSumation f7 = new FactorSumation(net.getNode("ALARM"), f4, f5, f6);
			
			var evidence1 = new []{new Evidence("MARYCALLS", "TRUE"), new Evidence("JOHNCALLS", "TRUE"), new Evidence("BURGLARY", "TRUE")};
			var evidence2 = new []{new Evidence("MARYCALLS", "TRUE"), new Evidence("JOHNCALLS", "TRUE"), new Evidence("BURGLARY", "FALSE")};

			var unNormalizedResultIfTrue = f1.solve(evidence1) * f7.solve(evidence1);
			var unNormalizedResultIfFalse = f1.solve(evidence2) * f7.solve(evidence2);
			var normalization = unNormalizedResultIfTrue + unNormalizedResultIfFalse;
			
			Console.WriteLine(unNormalizedResultIfTrue/normalization);
			Console.WriteLine(unNormalizedResultIfFalse/normalization);
		}
	}
}