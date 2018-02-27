// Syntactic Analyzer

// Legendary Syntactic Analyzer
// Date: 05-Mar-2018
// Authors:
//          A01375640 Brandon Alain Cruz Ruiz
//			A01376200 Oscar Allan Ruiz Toledo
//			A01374648 Mario Lagunes Nava
// File name: LegendarySyntacticAnalyzer.cs

/*
	program -> def-list EOF									expr-and -> expr-comp(&& expr-comp)*
	def-list -> def*										expr-comp -> expr-rel(op-comp expr-rel)*
	def -> (var-def | fun-def)+								expr-rel -> expr-add(op-rel expr-add)*
	var-def -> var var-list;								expr-add -> expr-mul(op-add expr-mul)*
	var-list -> id-list										expr-mul -> expr-unary(op-mul expr-unary)*
	id-list -> id id-list-cont								expr-unary -> expr-primary(op-unary exper-unary)*
	id-list-cont-> (, id id-list-cont)*						expr-primary -> func-call | id | array | lit | (expr)
	fun-def -> id(param-list){								array -> [expr-list]
				var-def-list								lit -> lit int | lit-char | lit str
				stmt-list}									op-unary -> + | - |!
	param-list -> id-list?									op-mul -> * | / | %
	var-def-list -> var-def*								op-add -> + | -
	stmt-list -> stmt*										op-rel -> < | <= | > | >=
	stmt -> stmt-assign | stmt-incr							op-comp -> == | !=
			stmt-decr | stmt-func-call						stmt-if -> if(expr){stmt-list}
			stmt-if | stmt-loop											else-if-list else
			stmt-break | stmt-return						else-if-list -> (elseif (expr) { stmt-list})?
			stmt-empty										else -> (else { stmt-list})?
	stmt-assig -> id = expr;								stmt-loop -> loop { stmt-list}
	stmt-incr -> id ++;										stmt-break -> break;
	stmt-decr -> id --;										stmt-return -> return expr;
	stmt-func-call -> func-call;							stmt-empty -> ;
	func-call -> id(expr-list)
	exper-list -> (expr expr-list-cont)?
	exper-list-cont -> (, expr expr-list-cont)*
	expr -> expr-or
	expr-or -> expr-and (|| expr-and)*
	
*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;


namespace DeepLingo {

	public class LegendarySyntacticAnalyzer {

		private IEnumerator<Token> stream;
		private Token previous;

		private static TokenCategory[] stmtCategories = {
			TokenCategory.BREAK, 
			TokenCategory.RETURN, 
			TokenCategory.IDENTIFIER, 
			TokenCategory.IF, 
			TokenCategory.LOOP, 
			TokenCategory.SEMICOLON};
		
		private static TokenCategory[] opRelCategories = {
			TokenCategory.LESS,
			TokenCategory.LESS_EQUAL,
			TokenCategory.GREATER,
			TokenCategory.GREATER_EQUAL
		};


		public static void Main(string[] args) {

			if (args.Length != 1) {
				Console.WriteLine("Incorrect number of arguments.");
				Environment.Exit(1);
			}
			var syntacticAnalyzer = new LegendarySyntacticAnalyzer (FileReader (args [0]));
			syntacticAnalyzer.Program ();
			Console.WriteLine ("Syntax ok!");
				
		}

		public static Reader FileReader(string archivo){
			Reader reader = null;
			try {     
				var str = File.ReadAllText (archivo);
				reader = new Reader(str);
			} catch (FileNotFoundException e) {
				Console.Error.WriteLine (e.Message);
				Environment.Exit (1);
			}

			return reader;

		}

		public LegendarySyntacticAnalyzer(Reader reader) {
			this.stream = reader.Start().GetEnumerator();
			this.stream.MoveNext ();
		}

		public TokenCategory Current {
			get { return stream.Current.Category; }
		}

		public Token Expect(TokenCategory category) {
			if (Current == category) {
				previous = stream.Current;
				stream.MoveNext ();
				return previous;
			} else {
				throw new SyntaxError(String.Format("Syntax Error: Expected {0}, given '{1}' at ({2}, {3})", ErrorFormat(category.ToString()), stream.Current.Value , previous.Row, previous.LastIndex()));
			}
		}

		public string ErrorFormat(string str) {
			str = str.Replace ('_', ' ');
			return str.ToLowerInvariant ();
		}

		public void Program() {
			DefList ();
			Expect (TokenCategory.EOF);
		}

		public void DefList() {
			if (Current == TokenCategory.VAR || Current == TokenCategory.IDENTIFIER) {
				Def ();	
			}
		}

		public void Def() {
			do {
				if (Current == TokenCategory.VAR) {
					VarDef();
				} else if(Current == TokenCategory.IDENTIFIER){
					FuncDef();
				}

			} while(Current == TokenCategory.VAR || Current == TokenCategory.IDENTIFIER);
		}

		public void VarDef() {
			Expect (TokenCategory.VAR);
			VarList ();
			Expect (TokenCategory.SEMICOLON);
		}

		public void VarList() {
			IdList ();
		}
			
		public void IdList() {
			Expect (TokenCategory.IDENTIFIER);
			IdListCont();
		}

		public void IdListCont() {
			if (Current == TokenCategory.COMMA) {
				Expect (TokenCategory.COMMA);
				Expect (TokenCategory.IDENTIFIER);
				IdListCont();
			}
		}

		public void FuncDef() {
			Expect(TokenCategory.IDENTIFIER);
			Expect (TokenCategory.OPEN_PARENTHESIS);
			ParamList ();
			Expect (TokenCategory.CLOSE_PARENTHESIS);
			Expect (TokenCategory.OPEN_BRACKET);
			VarDefList ();
			StmtList ();
			Expect (TokenCategory.CLOSE_BRACKET);

		}

		public void ParamList() {
			if (Current == TokenCategory.IDENTIFIER) {
				IdList ();
			}
		}
			
		public void VarDefList () {
			while (Current == TokenCategory.VAR) {
				VarDef ();
			}
		}

		public void StmtList() {
			while (System.Array.Exists(stmtCategories, category => category == Current)) {
				switch (Current) {
				case TokenCategory.IDENTIFIER:
					Expect (TokenCategory.IDENTIFIER);
					switch (Current) {
					case TokenCategory.ASSIGN:
						StmtAssign ();
						break;
					case TokenCategory.INCREMENT:
						StmtIncr ();
						break;
					case TokenCategory.DECREMENT:
						StmtDecr ();
						break;
					case TokenCategory.OPEN_PARENTHESIS:
						StmtFunCall ();
						break;
					}
					break;
				case TokenCategory.IF:
					StmtIf ();
					break;
				case TokenCategory.LOOP:
					StmtLoop ();
					break;
				case TokenCategory.BREAK:
					StmtBreak ();
					break;
				case TokenCategory.RETURN:
					StmtReturn();
					break;
				case TokenCategory.SEMICOLON:
					StmtEmpty ();
					break;
				}
			}
		}


		public void StmtAssign() {
			Expect (TokenCategory.ASSIGN);
			Expr ();
			Expect (TokenCategory.SEMICOLON);
		}

		public void StmtIncr() {
			Expect (TokenCategory.INCREMENT);
			Expect (TokenCategory.SEMICOLON);
		}

		public void StmtDecr() {
			Expect (TokenCategory.DECREMENT);
			Expect (TokenCategory.SEMICOLON);
		}

		public void StmtFunCall() {
			FunCall ();
			Expect (TokenCategory.SEMICOLON);
		}

		public void StmtIf() {
			Expect (TokenCategory.IF);
			Expect (TokenCategory.OPEN_PARENTHESIS);
			Expr ();
			Expect (TokenCategory.CLOSE_PARENTHESIS);
			Expect (TokenCategory.OPEN_BRACKET);
			StmtList ();
			Expect (TokenCategory.CLOSE_BRACKET);
			ElseIfList ();
			Else ();
		}

		public void StmtLoop() {
			Expect (TokenCategory.LOOP);
			Expect (TokenCategory.OPEN_BRACKET);
			StmtList ();
			Expect (TokenCategory.CLOSE_BRACKET);
		}

		public void StmtBreak()	{
			Expect (TokenCategory.BREAK);
			Expect (TokenCategory.SEMICOLON);
		}

		public void StmtReturn() {
			Expect (TokenCategory.RETURN);
			Expr ();
			Expect (TokenCategory.SEMICOLON);
		}

		public void StmtEmpty() {
			Expect (TokenCategory.SEMICOLON);
		}

		public void ElseIfList() {
			if (Current == TokenCategory.ELSEIF) {
				Expect (TokenCategory.ELSEIF);
				Expect (TokenCategory.OPEN_PARENTHESIS);
				Expr ();
				Expect (TokenCategory.CLOSE_PARENTHESIS);
				Expect (TokenCategory.OPEN_BRACKET);
				StmtList ();
				Expect (TokenCategory.CLOSE_BRACKET);
			}
		}

		public void Else() {
			if (Current == TokenCategory.ELSE) {
				Expect (TokenCategory.ELSE);
				Expect (TokenCategory.OPEN_BRACKET);
				StmtList ();
				Expect (TokenCategory.CLOSE_BRACKET);
			}
		}

		public void Expr() {
			ExprOr ();
		}

		public void ExprOr() {
			ExprAnd ();
			while (Current == TokenCategory.OR) {
				Expect (TokenCategory.OR);
				ExprAnd ();
			}
		}

		public void ExprAnd() {
			ExprComp ();
			while (Current == TokenCategory.AND) {
				Expect (TokenCategory.AND);
				ExprComp ();
			}
		}

		public void ExprComp() {
			ExprRel ();
			while (Current == TokenCategory.EQUAL || Current == TokenCategory.NOT_EQUAL) {
				OpComp ();
				ExprRel ();
			}
		}

		public void OpComp() {
			Expect(Current == TokenCategory.EQUAL ? TokenCategory.EQUAL : TokenCategory.NOT_EQUAL);
		}

		public void ExprRel() {
			ExprAdd ();
			while (System.Array.Exists (opRelCategories, category => category == Current)) {
				OpRel ();
				ExprAdd ();
			}

		}


		public void OpRel() {
			switch (Current) {
			case TokenCategory.LESS:
				Expect (TokenCategory.LESS);
				break;
			case TokenCategory.LESS_EQUAL:
				Expect (TokenCategory.LESS_EQUAL);
				break;
			case TokenCategory.GREATER:
				Expect (TokenCategory.GREATER);
				break;
			case TokenCategory.GREATER_EQUAL:
				Expect (TokenCategory.GREATER_EQUAL);
				break;
			default:
				break;
			}
		}


		public void ExprAdd() {
			ExprMul ();
			while (Current == TokenCategory.PLUS || Current == TokenCategory.MINUS) {
				OpAdd ();
				ExprMul ();
			}
		}

		public void OpAdd() {
			Expect (Current == TokenCategory.PLUS ? TokenCategory.PLUS : TokenCategory.MINUS);
		}

		public void ExprMul() {
			ExprUnary ();
			while (Current == TokenCategory.MUL || Current == TokenCategory.DIV || Current == TokenCategory.MOD) {
				OpMul ();
				ExprUnary ();
			}
		}

		public void OpMul() {
			if (Current == TokenCategory.MUL) {
				Expect (TokenCategory.MUL);
			} else if (Current == TokenCategory.DIV) {
				Expect (TokenCategory.DIV);
			} else if (Current == TokenCategory.MOD) {
				Expect (TokenCategory.MOD);
			}
		}

		public void ExprUnary() {
			if (Current == TokenCategory.PLUS || Current == TokenCategory.MINUS || Current == TokenCategory.NEG) {
				OpUnary ();
				ExprUnary ();
			} else {
				ExprPrimary ();
			}

		}

		public void OpUnary() {
			if (Current == TokenCategory.PLUS) {
				Expect (TokenCategory.PLUS);
			} else if (Current == TokenCategory.MINUS) {
				Expect (TokenCategory.MINUS);
			} else if (Current == TokenCategory.NEG) {
				Expect (TokenCategory.NEG);
			}
		}

		public void ExprPrimary() {
			switch (Current) {
			case TokenCategory.IDENTIFIER:
				Expect (TokenCategory.IDENTIFIER);
				if (Current == TokenCategory.OPEN_PARENTHESIS) {
					FunCall ();
				}
				break;
			case TokenCategory.OPEN_SQUARE_BRACK:
				Array ();
				break;
			case TokenCategory.OPEN_PARENTHESIS:
				Expr ();
				Expect (TokenCategory.CLOSE_PARENTHESIS);
				break;
			case TokenCategory.INTEGER_LITERAL:
			case TokenCategory.STRING_LITERAL:
			case TokenCategory.CHARACTER_LITERAL:
				Lit ();
				break;
			default:
				break;
			}
		}

		public void Lit() {
			if (Current == TokenCategory.INTEGER_LITERAL) {
				Expect (TokenCategory.INTEGER_LITERAL);
			} else if (Current == TokenCategory.STRING_LITERAL) {
				Expect (TokenCategory.STRING_LITERAL);
			} else {
				Expect (TokenCategory.CHARACTER_LITERAL);
			}


		}
			
		public void Array() {
			Expect (TokenCategory.OPEN_SQUARE_BRACK);
			ExprList ();
			Expect (TokenCategory.CLOSE_SQUARE_BRACK);
		}

		public void FunCall() {
			Expect (TokenCategory.OPEN_PARENTHESIS);
			ExprList ();
			Expect (TokenCategory.CLOSE_PARENTHESIS);
		}

		public void ExprList() {
			if (Current != TokenCategory.CLOSE_PARENTHESIS) {
				Expr ();
				ExprListCont ();
			}
		}


		public void ExprListCont() {
			if (Current == TokenCategory.COMMA) {
				Expect (TokenCategory.COMMA);
				Expr ();
				ExprListCont ();
			}
		}
	}

}