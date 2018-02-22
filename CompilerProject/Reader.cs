// This class performs the lexical analysis.

// Lexical Analysis
// Date: 29-Jan-2018
// Authors:
//          A01374648 Mario Lagunes Nava 
//          A01375640 Brandon Alain Cruz Ruiz
//			A01376200 Oscar Allan Ruiz Toledo
// File name: Reader.cs

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DeepLingo {

	public class Reader {

		string text;

		// *** Initializes a constructor for the class ***\\
		public Reader(string text){
			this.text = text;
		}

		// *** All the regular expressions and tokens ***\\
		static readonly Regex regex = new Regex(
			@"(?<StringLit>	"".*""	)
			|	(?<Comment>		(\/[*](.|\n)*[*]\/ | \/\/.*)	)
			|	(?<CharLit>		[']( \w | [\\] (n | r | [\\] | [""]| ['] | t | u[A-Fa-f0-9]{6} ) )[']	)
			|	(?<Identifier> 	[a-zA-Z][a-zA-Z0-9_]*	)
			|	(?<IntLit>		-?\d+ )
			|	(?<And>			([&][&]| [&])	)
			|	(?<Or>			([|][|] | [|])	)
			|	(?<Equals> 		==	)
			|	(?<GrEqu>		>=	)
			|	(?<LeEqu>		<=	)
			|	(?<NotEqu>		!=	)
			|	(?<PlusP>		[+][+]	)
			|	(?<MinusM>		--	)
			|	(?<Neg>			!	)
			|	(?<Assign>		=	)
			|	(?<RighBrack> 	{	)
			|	(?<LeftBrack> 	}	)
			| 	(?<Less>       [<]	)
			|	(?<Greater>		[>]	)
			| 	(?<Mul>        [*]	)
			| 	(?<Minus>        [-]	)
			|	(?<Div>		   [/]	)
			|	(?<Mod>		   [%]	)
			| 	(?<Newline>    \n 	)
			|	(?<ParLeft>    [(]	)
			| 	(?<ParRight>   [)]	)
			|	(?<SqBrackLeft>	\[	)
			|	(?<SqBrackRight>	\])
			| 	(?<Plus>       [+]	)
			|	(?<SemiColon>	;	)
			|	(?<Comma>		,	)
			|	(?<WhiteSpace>	\s	)
			|	(?<Other>		.	)", 
			RegexOptions.IgnorePatternWhitespace 
			| RegexOptions.Compiled
			| RegexOptions.Multiline);

		
		// *** A dictionary for the keywords ***\\
		static IDictionary<string, TokenCategory> keywords = new Dictionary<string, TokenCategory> () {
			{"if", TokenCategory.IF},
			{"var", TokenCategory.VAR},
			{"loop", TokenCategory.LOOP},
			{"else", TokenCategory.ELSE},
			{"elseif", TokenCategory.ELSEIF},
			{"break", TokenCategory.BREAK},
			{"return", TokenCategory.RETURN}
		};

		// *** A dictionary for the non-keywords ***\\
		static IDictionary<string, TokenCategory> nonkeywords = new Dictionary<string, TokenCategory> () {
			{"StringLit", TokenCategory.STRING_LITERAL},
			{"CharLit", TokenCategory.CHARACTER_LITERAL},
			{"Identifier", TokenCategory.IDENTIFIER},
			{"IntLit", TokenCategory.INTEGER_LITERAL},
			{"And", TokenCategory.AND},
			{"Or", TokenCategory.OR},
			{"Equals", TokenCategory.EQUAL},
			{"GrEqu", TokenCategory.GREATER_EQUAL},
			{"LeEqu", TokenCategory.LESS_EQUAL},
			{"NotEqu", TokenCategory.NOT_EQUAL},
			{"PlusP", TokenCategory.INCREMENT},
			{"MinusM", TokenCategory.DECREMENT},
			{"Assign", TokenCategory.ASSIGN},
			{"RighBrack", TokenCategory.OPEN_BRACKET},
			{"LeftBrack", TokenCategory.CLOSE_BRACKET},
			{"Less", TokenCategory.LESS},
			{"Greater", TokenCategory.GREATER},
			{"Mul", TokenCategory.MUL},
			{"Minus", TokenCategory.MINUS},
			{"Div", TokenCategory.DIV},
			{"Mod", TokenCategory.MOD},
			{"ParLeft", TokenCategory.OPEN_PARENTHESIS},
			{"ParRight", TokenCategory.CLOSE_PARENTHESIS},
			{"Plus", TokenCategory.PLUS},
			{"SemiColon", TokenCategory.SEMICOLON},
			{"Comma", TokenCategory.COMMA},
			{"Other", TokenCategory.ILL_CHAR},
			{"Neg", TokenCategory.NEG},
			{"SqBrackLeft", TokenCategory.OPEN_SQUARE_BRACK},
			{"SqBrackRight", TokenCategory.CLOSE_SQUARE_BRACK}

		};

		// *** Checks if the match is a newLine, comment, keyword, etc. ***\\
		public IEnumerable<Token> Start(){
			var row = 1;
			var column = 0;

			Func<Match, TokenCategory, Token> newTok = (m, tc) =>
				new Token(tc, m.Value, row, m.Index - column + 1);

			foreach (Match m in regex.Matches(this.text)) {

				if (m.Groups["Newline"].Success) {
					row++;
					column = m.Index + m.Length;
				} else if (m.Groups["WhiteSpace"].Success) {

				} else if (m.Groups["Comment"].Success) {
					int count = Reader.Count(m.Value, '\n');
					if (count > 0) {
						row += count;
					}

				} else {

					if (keywords.Keys.Contains (m.Value)) {
						yield return newTok (m, keywords [m.Value]);
					} else {
						foreach (var name in nonkeywords.Keys) {
							if (m.Groups[name].Success) {
								yield return newTok(m, nonkeywords[name]);
								break;
							}
						}
					}
				}
			}

			yield return new Token(TokenCategory.EOF, "", 0, 0);
		}

		// *** Counts the number of times a specific character appears in each line ***\\
		public static int Count(string str, char ch){
			char[] caracteres = str.ToCharArray ();
			int count = 0;
			foreach (char c in caracteres) {
				if (c == ch)
					count++;
			}				
			return count;
		}
		 
	}

}



