using System;
using System.Diagnostics.Eventing.Reader;

namespace BaesianNetworks {
	public class ParceManager {

		private string baseString;
		private int current = 0;
		private int last = 0;

		public ParceManager(string baseString) {
			this.baseString = baseString;
		}

		public string getHead() {
			if (last == current) return "";
			return baseString.Substring(last, current - last);
		}

		public string until(char c) {
			char cc;
			if(hasNext())
			do {
				next();
				cc = baseString[current];
			} while (hasNext() && cc != c);

			return getHead();
		}

		public char getCurrent() {
			return baseString[current];
		}
		
		public char getLast() {
			return baseString[last];
		}

		public bool isCurrentWhitesapce() {
			return Char.IsWhiteSpace(baseString, current);
		}
		
		public bool isLastWhitesapce() {
			return Char.IsWhiteSpace(baseString, last);
		}

		public string next() {
			current++;
			return getHead();
		}

		public string peek(int amount) {
			return baseString.Substring(current, amount);
		}

		public string mark() {
			string head = getHead();
			last = current + 1;
			current++;
			return head;
		}

		public bool hasNext() {
			return current < baseString.Length - 1;
		}

		public int lastIndex() {
			return last;
		}

		public int currentIndex() {
			return current;
		}

		public int headLength() {
			return current - last;
		}
	}
}