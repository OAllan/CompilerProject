// Token class for the scaneer.

// Token class
// Date: 29-Jan-2018
// Authors:
//          A01374648 Mario Lagunes Nava 
//          A01375640 Brandon Alain Cruz Ruiz
//			A01376200 Oscar Allan Ruiz Toledo
// File name: Token.cs

using System;

namespace DeepLingo {
	public class Token {

		private TokenCategory category;
		private string value;
		private int row, column;

		public Token(TokenCategory category, string value, int row, int column){
			this.category = category;
			this.value = value;
			this.row = row;
			this.column = column;
		}

		public override string ToString ()
		{
			return string.Format ("[{0}, {1}, @({2}, {3})]", category.ToString(), value, row, column);
		}

		public TokenCategory Category {
			get{ return this.category; }
		}

		public int Row {
			get{ return this.row; }
		}

		public int Column {
			get{ return this.column; }
		}


		public int LastIndex() {
			return column + value.Length;
		}

		public string Value{
			get{ return this.value; }
		}
	}
}
