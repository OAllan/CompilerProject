// Syntactic Analyzer

// Legendary Syntactic Analyzer
// Date: 29-Jan-2018
// Authors:
//          A01375640 Brandon Alain Cruz Ruiz
//			A01376200 Oscar Allan Ruiz Toledo
// File name: LegendarySyntacticAnalyzer.cs
using System.Collections.Generic;


namespace DeepLingo {

	public class LegendarySyntacticAnalyzer {

		private IEnumerator<Token> stream;

		public LegendarySyntacticAnalyzer(Reader reader) {
			this.stream = reader.Start();
		}

		public TokenCategory Current {
			get { return stream.Current.Category; }
		}

		public Token Expect(TokenCategory category) {
			if (Current == category) {
				Token current = stream.Current;
				stream.MoveNext ();
				return current;
			} else {
				throw new SyntaxError ();
			}
		}

		public void Program() {
			DefList ();
			Expect (TokenCategory.EOF);
		}

		public void DefList() {

			while (Current == TokenCategory.VAR || Current == TokenCategory.IDENTIFIER) {
				
			}
		}







	}

}