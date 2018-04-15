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