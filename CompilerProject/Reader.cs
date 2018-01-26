using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace DeepLingo {

	public class Reader {

		public static void Main(){
			var regex = new Regex(
			  @"(?<StringLit>	"".*""	)
			|	(?<Comment>		(\/[*](.|\n)*[*]\/|\/\/.*)	)
			|	(?<CharLit>		[']( \w | [\\](n | r | t| u[A-Fa-f0-9]{6} ) )[']	)
			|	(?<Identifier> 	[a-zA-Z][a-zA-Z0-9_]*	)
			|	(?<IntLit>		-?\d+ )
			|	(?<And>			([&][&]| [&])	)
			|	(?<Equals> 		==	)
			|	(?<GrEqu>		>=	)
			|	(?<LeEqu>		<=	)
			|	(?<NotEqu>		!=	)
			|	(?<PlusP>		[+][+]	)
			|	(?<MinusM>		--	)
			|	(?<Assign>		=	)
			|	(?<RighBrack> 	{	)
			|	(?<LeftBrack> 	}	)
			| 	(?<Less>       [<]	)
            | 	(?<Mul>        [*]	)
            | 	(?<Minus>        [-]	)
			|	(?<Div>		   [/]	)
			|	(?<Mod>		   [%]	)
			| 	(?<Newline>    \n 	)
            |	(?<ParLeft>    [(]	)
            | 	(?<ParRight>   [)]	)
            | 	(?<Plus>       [+]	)
			|	(?<SemiColon>	;	)
			|	(?<Comma>		,	)", 
				RegexOptions.IgnorePatternWhitespace 
				| RegexOptions.Compiled
				| RegexOptions.Multiline);

			try {                 
				var str = File.ReadAllText("binary.deep");
				foreach (Match m in regex.Matches(str)) {

					if (m.Groups["Newline"].Success) {


					} else if (m.Groups["WhiteSpace"].Success) {

						// Skip white space and comments.

					} else if (m.Groups["Identifier"].Success) {

						Console.WriteLine("Identifier: " + m.Value);

					} else if (m.Groups["Comment"].Success) {

						Console.WriteLine("Comment: " + m.Value);

					} else if (m.Groups["StringLit"].Success) {

						Console.WriteLine("String: " + m.Value);

					} else if (m.Groups["CharLit"].Success) {

						Console.WriteLine("Character: " + m.Value);

					} else {

						Console.WriteLine ("Other: " + m.Value);
					}
				}


			} catch (FileNotFoundException e) {
				Console.Error.WriteLine(e.Message);
				Environment.Exit(1);
			}      

		}
				
	}


}



