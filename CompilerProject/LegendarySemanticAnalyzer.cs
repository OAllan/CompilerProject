// This class performs the semantic analysis.

// Semantic Analysis
// Date: 16-April-2018
// Authors:
//          A01374648 Mario Lagunes Nava 
//          A01375640 Brandon Alain Cruz Ruiz
//          A01376200 Oscar Allan Ruiz Toledo
// File name: LegendarySemanticAnalyzer.cs

using System;
using System.Collections.Generic;

namespace DeepLingo {
    public class LegendarySemanticAnalyzer {

        public SymbolTable Variables {
            get;
            private set;
        }

        public FunctionTable Functions {
            get;
            private set;
        }

        private string Scope;

        public LegendarySemanticAnalyzer() {
            Functions = new FunctionTable();
            Functions["printi"] = 1;
            Functions["printc"] = 1;
            Functions["prints"] = 1;
            Functions["println"] = 0;
            Functions["readi"] = 0;
            Functions["reads"] = 0;
            Functions["new"] = 1;
            Functions["size"] = 1;
            Functions["add"] = 2;
            Functions["get"] = 2;
            Functions["set"] = 3;
            Functions["sizeof"] = 1;
            Variables = new SymbolTable();
            Scope = "global";
        }

        public void Visit (Program node) {
            VisitChildren(node[0]);
            if (!Functions.Contains("main")) {
                throw new SemanticError("Function main not declared");
            }
            if (Functions.Contains("main") && Functions["main"] != 0){
                throw new SemanticError("Function main must have arity 0");
            }
            Scope = "";
            foreach (var n in node[0]){
                if (n.GetType() == typeof(VarDefinition)) {
                    continue;
                }
                Visit((dynamic) n);
            }
        }

        public void Visit (VarDefinition node) {
            var identifierList = node[0];
            foreach (var n in identifierList){
                var id = n.AnchorToken.Value;
                if(Variables.Contains(id, Scope)) {
                    throw new SemanticError("Duplicated variable '" + id + "' on " + Scope, n.AnchorToken);
                }
                Variables.Add(new Variable(id, Scope));
            }
        }

        public void Visit (FunctionDefinition node) {
            if (!Scope.Equals("global")) {
                Scope = node.AnchorToken.Value;
                Visit ((dynamic) node[0]);
                Visit ((dynamic) node[1]);
                Visit ((dynamic) node[2]);
            } else {
                if (Functions.Contains(node.AnchorToken.Value)){
                    throw new SemanticError("Function '" + node.AnchorToken.Value + "' already defined!", node.AnchorToken);
                }
                Functions[node.AnchorToken.Value] = node[0].ChildrenSize();
            }
        }

        public void Visit (VarDefinitionList node) {
            VisitChildren(node);
        }

        public void Visit (ParameterList node) {
            foreach (var n in node) {
                var id = n.AnchorToken.Value;
                if(Variables.Contains(id, Scope)){
                    throw new SemanticError("Duplicated variable '" + id + "' on " + Scope, n.AnchorToken);
                }
                Variables.Add(new Variable(id, Scope));
            }
        }

        public void Visit (StatementList node) {
            VisitChildren(node);
        } 

        public void Visit (If node) {
            VisitChildren(node);
        }

        public void Visit (NotEqual node) {
            VisitChildren(node);
        }

        public void Visit (Assignment node) {
            if (!Variables.Contains(node.AnchorToken.Value, Scope) && !Variables.Contains(node.AnchorToken.Value)) {
                throw new SemanticError ("Variable '" + node.AnchorToken.Value + "' not declared!", node.AnchorToken);
            }
            VisitChildren(node);
        }

        public void Visit (Plus node) {
            VisitChildren(node);
        }

        public void Visit (Minus node) {
            VisitChildren(node);
        }

        public void Visit (Greater node) { 
            VisitChildren(node);
        }

        public void Visit (GreaterEqual node) {
            VisitChildren(node);
        }

        public void Visit(Equal node) {
            VisitChildren(node);
        }

        public void Visit(Neg node) {
            VisitChildren(node);
        }

        public void Visit (Less node) {
            VisitChildren(node);
        }

        public void Visit (LessEqual node) {
            VisitChildren(node);
        }

        public void Visit (And node) {
            VisitChildren(node);
        }

        public void Visit (Or node) {
            VisitChildren(node);
        }

        public void Visit (Return node) {
            VisitChildren(node);
        }

        public void Visit (Array node) { 
            VisitChildren(node);
        }

        public void Visit (ExpressionList node) {
            VisitChildren(node);
        }

        public void Visit (FunctionCall node) {
            if(!Functions.Contains(node.AnchorToken.Value)) {
                throw new SemanticError("Function '"+ node.AnchorToken.Value + "' not declared!", node.AnchorToken);
            } else if (Functions[node.AnchorToken.Value] != node[0].ChildrenSize()) {
                throw new SemanticError("Incorrect arity for function '" + node.AnchorToken.Value + "'. Expected " + Functions[node.AnchorToken.Value] + " arguments, given " + node[0].ChildrenSize(), node.AnchorToken);
            }
            VisitChildren(node);
        }

        public void Visit (ElseIfList node) {
            VisitChildren (node);
        }

        public void Visit (ElseIf node) {
            VisitChildren(node);
        }

        public void Visit (Else node) {
            VisitChildren(node);
        }

        public void Visit (IntegerLiteral node) {
            try {
                Convert.ToInt32(node.AnchorToken.Value);
            } catch(Exception e){
                throw new SemanticError("Invalid integer literal", node.AnchorToken);
            }
        }

        public void Visit (CharacterLiteral node) {

        }

        public void Visit (StringLiteral node) {
            
        }

        public void Visit (Mul node) {
            VisitChildren(node);
        }

        public void Visit (Div node) {
            VisitChildren(node);
        }

        public void Visit (Mod node) {
            VisitChildren(node);
        }

        public void Visit (Identifier node) {
            var lexeme = node.AnchorToken.Value;
            if (!Variables.Contains(lexeme, Scope) && !Variables.Contains(lexeme)){
                throw new SemanticError("Variable '" + lexeme + "' not declared!", node.AnchorToken);
            }
        }

        public void Visit (Break node) {
            if (!isInLoop()) {
                throw new SemanticError("Break statement must be inside a loop", node.AnchorToken);
            }
        }

        public void Visit (Loop node) { 
            Scope += "-loop";
            VisitChildren(node);
            var index = Scope.LastIndexOf('-');
            Scope = Scope.Substring(0, index);
        }

        public void Visit (Increment node) {
            var lexeme = node.AnchorToken.Value;
            if (!Variables.Contains(lexeme, Scope) && !Variables.Contains(lexeme)){
                throw new SemanticError("Variable '" + lexeme + "' not declared!", node.AnchorToken);
            }
        }


        public void Visit (Decrement node) {
            var lexeme = node.AnchorToken.Value;
            if (!Variables.Contains(lexeme, Scope) && !Variables.Contains(lexeme)){
                throw new SemanticError("Variable '" + lexeme + "' not declared!", node.AnchorToken);
            }
        }


        public bool isInLoop () {
            return Scope.Contains("-loop");
        }

        public void VisitChildren (Node node) {
            foreach (var n in node){
                Visit((dynamic) n);
            }
        }

    }
}