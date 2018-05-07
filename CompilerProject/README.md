Compiler Project
========

## Authors
	A01374648 Mario Lagunes Nava 
	A01375640 Brandon Alain Cruz Ruiz
	A01376200 Oscar Allan Ruiz Toledo

## Included in this release

   * Lexical analysis
   * Syntactic analysis
   * AST Construction
   * Semantic analysis
   * CIL Code Generation
    
## Lexical Analyzer

To build, type:
```
make LegendaryTokenAnalyzer.exe
```

To run:
```
mono LegendaryTokenAnalyzer.exe <file_name>
```

## Syntactic Analyzer and AST construction

To build, type:
```
make LegendarySyntacticAnalyzer.exe
```

To run:
```
mono LegendarySyntacticAnalyzer.exe <file_name>
```

## Semantic Analyzer

To build, type:
```
make LegendarySemanticAnalyzer.exe
```

To run:
```
mono LegendarySemanticAnalyzer.exe <file_name>

## CIL Code Generation

To build, type:
```
make Compiler.exe
```

To run:
```
mono Compiler.exe <file_name>
```

To try the generated code, type:
```
ilasm output.il
```

To run it, type:
```
mono output.exe
```

Where <file_name> is the name of a DeepLingo source file. You can try with
these files:

   * ultimate.deep
   * binary.deep
   * palindrome.deep
   * factorial.deep
   * arrays.deep
   * next_day.deep
   * literals.deep
