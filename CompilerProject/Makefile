# Make file

# Make file
# Date: 29-Jan-2018
# Authors:
#          A01374648 Mario Lagunes Nava 
#          A01375640 Brandon Alain Cruz Ruiz
#		   A01376200 Oscar Allan Ruiz Toledo
# File name: TokenCatergory.cs



LegendaryTokenAnalyzer.exe: Reader.cs LegendaryTokenAnalyzer.cs Token.cs TokenCategory.cs               
	mcs -out:LegendaryTokenAnalyzer.exe Reader.cs LegendaryTokenAnalyzer.cs Token.cs TokenCategory.cs

LegendarySyntacticAnalyzer.exe: Reader.cs LegendaryTokenAnalyzer.cs Token.cs TokenCategory.cs SyntaxError.cs
	mcs -out:LegendarySyntacticAnalyzer.exe Reader.cs LegendarySyntacticAnalyzer.cs Token.cs TokenCategory.cs SyntaxError.cs Node.cs NodeDefinitions.cs

LegendarySemanticAnalyzer.exe: Reader.cs LegendaryTokenAnalyzer.cs Token.cs TokenCategory.cs SyntaxError.cs
	mcs -out:LegendarySemanticAnalyzer.exe Reader.cs LegendarySyntacticAnalyzer.cs Token.cs TokenCategory.cs SyntaxError.cs Node.cs NodeDefinitions.cs LegendarySemanticAnalyzer.cs SymbolTable.cs FunctionTable.cs SemanticError.cs Variable.cs

Compiler.exe: Reader.cs LegendaryTokenAnalyzer.cs Token.cs TokenCategory.cs SyntaxError.cs
	mcs -out:Compiler.exe Reader.cs LegendarySyntacticAnalyzer.cs Token.cs TokenCategory.cs SyntaxError.cs Node.cs NodeDefinitions.cs LegendarySemanticAnalyzer.cs SymbolTable.cs FunctionTable.cs SemanticError.cs Variable.cs LegendaryCompiler.cs

clean:
	rm LegendaryTokenAnalyzer.exe
