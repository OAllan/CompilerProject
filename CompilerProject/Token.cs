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


	}
}
