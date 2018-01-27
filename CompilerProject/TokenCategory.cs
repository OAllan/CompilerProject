// Token categories for the scaneer.

// Token categories
// Date: 29-Jan-2018
// Authors:
//          A01374648 Mario Lagunes Nava 
//          A01375640 Brandon Alain Cruz Ruiz
//			A01376200 Oscar Allan Ruiz Toledo
// File name: TokenCatergory.cs

namespace DeepLingo {

	public enum TokenCategory{
		STRING_LITERAL,
		CHARACTER_LITERAL,
		IDENTIFIER,
		INTEGER_LITERAL,
		AND,
		OR,
		EQUAL,
		GREATER_EQUAL,
		LESS_EQUAL,
		NOT_EQUAL,
		INCREMENT,
		DECREMENT,
		ASSIGN,
		OPEN_BRACKET,
		CLOSE_BRACKET,
		LESS,
		GREATER,
		MUL,
		MINUS,
		DIV,
		MOD, 
		OPEN_PARENTHESIS,
		CLOSE_PARENTHESIS,
		PLUS,
		SEMICOLON, 
		COMMA,
		ILL_CHAR,
		BREAK,
		LOOP,
		IF,
		ELSE,
		ELSEIF,
		RETURN,
		VAR
	}
}
