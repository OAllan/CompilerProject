/* Syntactic Analyzer
*
*   Legendary Syntactic Analyzer
*   Date: 05-Mar-2018
*   Authors:
*           A01375640 Brandon Alain Cruz Ruiz
*           A01376200 Oscar Allan Ruiz Toledo
*           A01374648 Mario Lagunes Nava
*   File name: LegendarySyntacticAnalyzer.cs
*
*
*	program -> def-list EOF									expr-and -> expr-comp(&& expr-comp)*
*	def-list -> def+										expr-comp -> expr-rel(op-comp expr-rel)*
*	def -> (var-def | fun-def)?								expr-rel -> expr-add(op-rel expr-add)*
*	var-def -> var var-list;								expr-add -> expr-mul(op-add expr-mul)*
*	var-list -> id-list										expr-mul -> expr-unary(op-mul expr-unary)*
*	id-list -> id id-list-cont								expr-unary -> expr-primary(op-unary exper-unary)*
*	id-list-cont-> (, id id-list-cont)*						expr-primary -> func-call | id | array | lit | (expr)
*	fun-def -> id(param-list){								array -> [expr-list]
*				var-def-list								lit -> lit int | lit-char | lit str
*				stmt-list}									op-unary -> + | - |!
*	param-list -> id-list?									op-mul -> * | / | %
*	var-def-list -> var-def*								op-add -> + | -
*	stmt-list -> stmt*										op-rel -> < | <= | > | >=
*	stmt -> id (stmt-assign | stmt-incr							op-comp -> == | !=
*			stmt-decr | stmt-func-call) |						stmt-if -> if(expr){stmt-list}
*			stmt-if | stmt-loop											else-if-list else
*			stmt-break | stmt-return						else-if-list -> (elseif (expr) { stmt-list})*
*			stmt-empty										else -> (else { stmt-list})?
*	stmt-assig -> = expr;								stmt-loop -> loop { stmt-list}
*	stmt-incr -> ++;										stmt-break -> break;
*	stmt-decr -> --;										stmt-return -> return expr;
*	stmt-func-call -> func-call;							stmt-empty -> ;
*	func-call -> (expr-list)
*	exper-list -> (expr expr-list-cont)?
*	exper-list-cont -> (, expr expr-list-cont)*
*	expr -> expr-or
*	expr-or -> expr-and (|| expr-and)*
*	
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
			var program = syntacticAnalyzer.Program ();
			Console.WriteLine (program.ToStringTree());
				
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

		public Node Program() {
			var program = new Program ();
			program.Add(DefList ());
			Expect (TokenCategory.EOF);
			return program;
		}

		public Node DefList() {
			var defList = new DefinitionList ();
			while (Current == TokenCategory.VAR || Current == TokenCategory.IDENTIFIER) {
				defList.Add(Def ());	
			}
			return defList;
		}

		public Node Def() {
			if (Current == TokenCategory.VAR) {
				return VarDef();
			} else {
				return FuncDef();
			}
		}

		public Node VarDef() {
			var varDefinition = new VarDefinition () {
				AnchorToken = Expect (TokenCategory.VAR)
			};
			varDefinition.Add(VarList ());
			Expect (TokenCategory.SEMICOLON);
			return varDefinition;
		}

		public Node VarList() {
			return IdList ();
		}
			
		public Node IdList(Node paramList = null) {
			var idList = paramList != null? paramList : new IdentifierList (); 
			var id = new Identifier () {
				AnchorToken = Expect (TokenCategory.IDENTIFIER)
			};
			idList.Add(id);
			IdListCont(idList);
			return idList;
		}

		public void IdListCont(Node idList) {
			if (Current == TokenCategory.COMMA) {
				Expect (TokenCategory.COMMA);
				var id = new Identifier () {
					AnchorToken = Expect (TokenCategory.IDENTIFIER)
				};
				idList.Add (id);
				IdListCont(idList);
			}
		}

		public Node FuncDef() {
			var functionDefinition = new FunctionDefinition () {
				AnchorToken = Expect (TokenCategory.IDENTIFIER)
			};
			Expect (TokenCategory.OPEN_PARENTHESIS);
			functionDefinition.Add (ParamList());
			Expect (TokenCategory.CLOSE_PARENTHESIS);
			Expect (TokenCategory.OPEN_BRACKET);
			functionDefinition.Add (VarDefList ());
			functionDefinition.Add (StmtList ());
			Expect (TokenCategory.CLOSE_BRACKET);
			return functionDefinition;
		}

		public Node ParamList() {
			var paramList = new ParameterList ();
			if (Current == TokenCategory.IDENTIFIER) {
				IdList (paramList);
			}
			return paramList;
		}
			
		public Node VarDefList () {
			var varDefinitionList = new VarDefinitionList ();
			while (Current == TokenCategory.VAR) {
				varDefinitionList.Add (VarDef ());
			}
			return varDefinitionList;
		}

		public Node StmtList() {
			var statementList = new StatementList ();
			while (System.Array.Exists(stmtCategories, category => category == Current)) {
				switch (Current) {
				case TokenCategory.IDENTIFIER:
					var id = Expect (TokenCategory.IDENTIFIER);
					switch (Current) {
					case TokenCategory.ASSIGN:
						statementList.Add(StmtAssign (id));
						break;
					case TokenCategory.INCREMENT:
						statementList.Add(StmtIncr (id));
						break;
					case TokenCategory.DECREMENT:
						statementList.Add(StmtDecr (id));
						break;
					case TokenCategory.OPEN_PARENTHESIS:
						statementList.Add(StmtFunCall (id));
						break;
					default:
						throw new SyntaxError(String.Format("Syntax Error: Expected operator or open parenthesis, given '{0}' at ({1}, {2})", stream.Current.Value , previous.Row, previous.LastIndex()));
					}
					break;
				case TokenCategory.IF:
					statementList.Add (StmtIf ());
					break;
				case TokenCategory.LOOP:
					statementList.Add (StmtLoop ());
					break;
				case TokenCategory.BREAK:
					statementList.Add (StmtBreak ());
					break;
				case TokenCategory.RETURN:
					statementList.Add (StmtReturn ());
					break;
				case TokenCategory.SEMICOLON:
					statementList.Add (StmtEmpty ());
					break;
				}
			}
			return statementList;
		}


		public Node StmtAssign(Token id) {
			var assignment = new Assignment () {
				AnchorToken = id
			};
			Expect (TokenCategory.ASSIGN);
			assignment.Add(Expr ());
			Expect (TokenCategory.SEMICOLON);
			return assignment;
		}

		public Node StmtIncr(Token id) {
			var increment = new Increment () {
				AnchorToken = id
			};
			Expect (TokenCategory.INCREMENT);
			Expect (TokenCategory.SEMICOLON);
			return increment;
		}

		public Node StmtDecr(Token id) {
			var decrement = new Decrement () {
				AnchorToken = id
			};
			Expect (TokenCategory.DECREMENT);
			Expect (TokenCategory.SEMICOLON);
			return decrement;
		}

		public Node StmtFunCall(Token id) {
			var funCall = new FunctionCall () {
				AnchorToken = id
			};
			funCall.Add(FunCall ());
			Expect (TokenCategory.SEMICOLON);
			return funCall;
		}

		public Node StmtIf() {
			var ifStatement = new If () {
				AnchorToken = Expect (TokenCategory.IF)
			};
			Expect (TokenCategory.OPEN_PARENTHESIS);
			ifStatement.Add(Expr ());
			Expect (TokenCategory.CLOSE_PARENTHESIS);
			Expect (TokenCategory.OPEN_BRACKET);
			ifStatement.Add (StmtList ());
			Expect (TokenCategory.CLOSE_BRACKET);
			ifStatement.Add (ElseIfList ());
			ifStatement.Add (Else ());
			return ifStatement;
		}

		public Node StmtLoop() {
			var loop = new Loop () {
				AnchorToken = Expect (TokenCategory.LOOP)
			};
			Expect (TokenCategory.OPEN_BRACKET);
			loop.Add(StmtList ());
			Expect (TokenCategory.CLOSE_BRACKET);
			return loop;
		}

		public Node StmtBreak()	{
			var breakStatement = new Break () {
				AnchorToken = Expect (TokenCategory.BREAK)
			};
			Expect (TokenCategory.SEMICOLON);
			return breakStatement;
		}

		public Node StmtReturn() {
			var returnStatement = new Return () {
				AnchorToken = Expect (TokenCategory.RETURN)
			};
			returnStatement.Add (Expr ());
			Expect (TokenCategory.SEMICOLON);
			return returnStatement;
		}

		public Node StmtEmpty() {
			Expect (TokenCategory.SEMICOLON);
			return new EmptyStatement();
		}

		public Node ElseIfList() {
			var elseIfList = new ElseIfList ();
			while (Current == TokenCategory.ELSEIF) {
				var elseIf = new ElseIf () {
					AnchorToken = Expect (TokenCategory.ELSEIF)
				};
				Expect (TokenCategory.OPEN_PARENTHESIS);
				elseIf.Add (Expr ());
				Expect (TokenCategory.CLOSE_PARENTHESIS);
				Expect (TokenCategory.OPEN_BRACKET);
				elseIf.Add (StmtList ());
				Expect (TokenCategory.CLOSE_BRACKET);
				elseIfList.Add (elseIf);
			}
			return elseIfList;
		}

		public Node Else() {
			var elseStatement = new Else ();
			if (Current == TokenCategory.ELSE) {
				elseStatement.AnchorToken = Expect (TokenCategory.ELSE);
				Expect (TokenCategory.OPEN_BRACKET);
				elseStatement.Add (StmtList ());
				Expect (TokenCategory.CLOSE_BRACKET);
				return elseStatement;
			}
			return elseStatement;
		}

		public Node Expr() {
			return ExprOr ();
		}

		public Node ExprOr() {
			var and = ExprAnd ();
			while (Current == TokenCategory.OR) {
				var or = new Or () {
					AnchorToken = Expect (TokenCategory.OR)
				};
				or.Add (and);
				or.Add (ExprAnd ());
				and = or;
			}
			return and;
		}

		public Node ExprAnd() {
			var comp = ExprComp ();
			while (Current == TokenCategory.AND) {
				var and = new And () {
					AnchorToken = Expect (TokenCategory.AND)
				};
				and.Add (comp);
				and.Add (ExprComp ());
				comp = and;
			}
			return comp;
		}

		public Node ExprComp() {
			var rel = ExprRel ();
			while (Current == TokenCategory.EQUAL || Current == TokenCategory.NOT_EQUAL) {
				var comp = OpComp ();
				comp.Add (rel);
				comp.Add (ExprRel ());
				rel = comp;
			}
			return rel;
		}

		public Node OpComp() {
			if (Current == TokenCategory.EQUAL) {
				return new Equal () {
					AnchorToken = Expect (TokenCategory.EQUAL)
				};
			} else {
				return new NotEqual () {
					AnchorToken = Expect (TokenCategory.NOT_EQUAL)
				};
			}
		}

		public Node ExprRel() {
			var add = ExprAdd ();
			while (System.Array.Exists (opRelCategories, category => category == Current)) {
				var rel = OpRel ();
				rel.Add (add);
				rel.Add (ExprAdd ());
				add = rel;
			}
			return add; 
		}


		public Node OpRel() {
			switch (Current) {
			case TokenCategory.LESS:
				return new Less () {
					AnchorToken = Expect (TokenCategory.LESS)
				};
			case TokenCategory.LESS_EQUAL:
				return new LessEqual () {
					AnchorToken = Expect (TokenCategory.LESS_EQUAL)
				};
			case TokenCategory.GREATER:
				return new Greater () {
					AnchorToken = Expect (TokenCategory.GREATER)
				};
			case TokenCategory.GREATER_EQUAL:
				return new GreaterEqual () {
					AnchorToken = Expect (TokenCategory.GREATER_EQUAL)
				};
			default:
				return null;
			}
		}


		public Node ExprAdd() {
			var mul = ExprMul ();
			while (Current == TokenCategory.PLUS || Current == TokenCategory.MINUS) {
				var add = OpAdd ();
				add.Add (mul);
				add.Add (ExprMul ());
				mul = add;
			}
			return mul;
		}

		public Node OpAdd() {
			if (Current == TokenCategory.PLUS) {
				return new Plus () {
					AnchorToken = Expect (TokenCategory.PLUS)
				};
			} else {
				return new Minus () {
					AnchorToken = Expect (TokenCategory.MINUS)
				};
			}
		}

		public Node ExprMul() {
			var unary = ExprUnary ();
			while (Current == TokenCategory.MUL || Current == TokenCategory.DIV || Current == TokenCategory.MOD) {
				var mul = OpMul ();
				mul.Add (unary);
				mul.Add (ExprUnary ());
				unary = mul; 
			}
			return unary;
		}

		public Node OpMul() {
			if (Current == TokenCategory.MUL) {
				return new Mul () {
					AnchorToken = Expect (TokenCategory.MUL)
				};
			} else if (Current == TokenCategory.DIV) {
				return new Div () {
					AnchorToken = Expect (TokenCategory.DIV)
				};
			} else {
				return new Mod () {
					AnchorToken = Expect (TokenCategory.MOD)
				};
			}
		}

		public Node ExprUnary() {
			if (Current == TokenCategory.PLUS || Current == TokenCategory.MINUS || Current == TokenCategory.NEG) {
				var opUnary = OpUnary ();
				opUnary.Add(ExprUnary ());
				return opUnary;
			} else {
				return ExprPrimary ();
			}
		}

		public Node OpUnary() {
			if (Current == TokenCategory.PLUS) {
				return new Plus() {
					AnchorToken = Expect (TokenCategory.PLUS)
				};
			} else if (Current == TokenCategory.MINUS) {
				return new Minus() {
					AnchorToken = Expect (TokenCategory.MINUS)
				};
			} else {
				return new Neg() {
					AnchorToken = Expect (TokenCategory.NEG)
				};
			}
		}

		public Node ExprPrimary() {
			switch (Current) {
			case TokenCategory.IDENTIFIER:
				var id = Expect (TokenCategory.IDENTIFIER);
				if (Current == TokenCategory.OPEN_PARENTHESIS) {
					var funCall = new FunctionCall () {
						AnchorToken = id
					};
					funCall.Add (FunCall ());
					return funCall;
				} else {
					return new Identifier () {
						AnchorToken = id
					};
				}
			case TokenCategory.OPEN_SQUARE_BRACK:
				return Array ();
			case TokenCategory.OPEN_PARENTHESIS:
				Expect (TokenCategory.OPEN_PARENTHESIS);
				var expr = Expr ();
				Expect (TokenCategory.CLOSE_PARENTHESIS);
				return expr;
			case TokenCategory.INTEGER_LITERAL:
			case TokenCategory.STRING_LITERAL:
			case TokenCategory.CHARACTER_LITERAL:
				return Lit ();
			default:
				throw new SyntaxError(String.Format("Syntax Error: Expected expression, given '{0}' at ({1}, {2})", stream.Current.Value , previous.Row, previous.LastIndex()));
			}
		}

		public Node Lit() {
			if (Current == TokenCategory.INTEGER_LITERAL) {
				return new IntegerLiteral () {
					AnchorToken = Expect (TokenCategory.INTEGER_LITERAL)
				};
			} else if (Current == TokenCategory.STRING_LITERAL) {
				return new StringLiteral () {
					AnchorToken = Expect (TokenCategory.STRING_LITERAL)
				};
			} else {
				return new CharacterLiteral () {
					AnchorToken = Expect (TokenCategory.CHARACTER_LITERAL)
				};
			}
		}
			
		public Node Array() {
			Expect (TokenCategory.OPEN_SQUARE_BRACK);
			var exprList = ExprList ();
			Expect (TokenCategory.CLOSE_SQUARE_BRACK);
			return exprList;
		}

		public Node FunCall() {
			Expect (TokenCategory.OPEN_PARENTHESIS);
			var exprList = ExprList ();
			Expect (TokenCategory.CLOSE_PARENTHESIS);
			return exprList;
		}

		public Node ExprList() {
			var exprList = new ExpressionList ();
			if (Current != TokenCategory.CLOSE_PARENTHESIS) {
				exprList.Add(Expr ());
				ExprListCont (exprList);
			}
			return exprList;
		}


		public void ExprListCont(Node exprList) {
			if (Current == TokenCategory.COMMA) {
				Expect (TokenCategory.COMMA);
				exprList.Add (Expr ());
				ExprListCont (exprList);
			}
		}
	}

}