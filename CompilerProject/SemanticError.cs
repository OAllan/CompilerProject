// This class performs the lexical analysis.

// Semantic Analysis
// Date: 16-April-2018
// Authors:
//          A01374648 Mario Lagunes Nava 
//          A01375640 Brandon Alain Cruz Ruiz
//          A01376200 Oscar Allan Ruiz Toledo
// File name: SemanticError.cs

using System;

namespace DeepLingo {
    class SemanticError: Exception {

        public SemanticError(string message, Token token):
            base(String.Format(
                "Semantic Error: {0} \n" +
                "at row {1}, column {2}.",
                message,
                token.Row,
                token.Column)) {
        }

        public SemanticError(string message): base(message) {
            
        }
    }
}