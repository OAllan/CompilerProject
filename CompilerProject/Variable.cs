// This class is used to create variables found in the program

// Semantic Analysis
// Date: 16-April-2018
// Authors:
//          A01374648 Mario Lagunes Nava 
//          A01375640 Brandon Alain Cruz Ruiz
//          A01376200 Oscar Allan Ruiz Toledo
// File name: Variable.cs

using System;

namespace DeepLingo {
    public class Variable {
        public string Identifier {
            get;
            private set;
        }

        public string Scope {
            get;
            private set;
        }

        public Variable(string Identifier, string Scope){
            this.Identifier = Identifier;
            this.Scope = Scope;
        }
    }
}