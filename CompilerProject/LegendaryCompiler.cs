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
    public class LegendaryCompiler {

        bool isGlobal;

        IDictionary<string, int> codePoints = new Dictionary<string, int>(){
            { @"\n", 10},
            { @"\r", 13},
            { @"\t", 9},
            { @"\\", 92},
            { @"\'", 39},
            { "\\\"", 34}
        };

        public SymbolTable Variables {
            get;
            private set;
        }

        public FunctionTable Functions {
            get;
            private set;
        }

        int labelCounter = 0;

        //-----------------------------------------------------------
        string GenerateLabel() {
            return String.Format("${0:000000}", labelCounter++);
        } 

        private string Scope;

        public LegendaryCompiler() {
            Functions = new FunctionTable();
            Functions["printi"] = 1;
            Functions["printc"] = 1;
            Functions["prints"] = 1;
            Functions["println"] = 1;
            Functions["readi"] = 0;
            Functions["reads"] = 0;
            Functions["new"] = 0;
            Functions["size"] = 0;
            Functions["add"] = 1;
            Functions["get"] = 0;
            Functions["set"] = 1;
            Variables = new SymbolTable();
            isGlobal = true;
            Scope = "global";
        }

        public string Visit (Program node) {
            return @"// CIL program.
.assembly extern 'deeplingolib' { }
.assembly 'compiler' { }
.class public 'DeepLingoCompiler' extends ['mscorlib']'System'.'Object' {
"
            + VisitVariable(node[0])
            + VisitFunction(node[0])
            + "}\n";
        }


        public string VisitVariable(Node node){
            string varList = "";
            foreach(var child in node){
                if (child.GetType() != typeof(VarDefinition)) {
                    continue;
                }
                varList += Visit((dynamic) child);
            }
            isGlobal = false;
            Scope = "";
            return varList;
        }

        public string VisitFunction(Node node){
            string functionList = "";
            foreach(var child in node){
                if (child.GetType() == typeof(VarDefinition)) {
                    continue;
                }
                functionList += Visit((dynamic) child);
            }
            return functionList;
        }

        public string Visit (VarDefinition node) {
            var result = "";
            if(!isGlobal){                
                foreach(var child in node[0]){
                    var varName = child.AnchorToken.Value;
                    Variables.Add(new Variable(varName, Scope));
                    result += "\t\t.locals init (int32 '" +  varName + "')\n";
                }
                return result;
                
            }
            foreach(var child in node[0]){
                var varName = child.AnchorToken.Value;
                Variables.Add(new Variable(varName, Scope));
                result += "\t.field private static int32 '" + varName + "'\n";
            }
            return result;
        }

        public string Visit (FunctionDefinition node) {
            Scope = node.AnchorToken.Value;
            var entryPoint = node.AnchorToken.Value.Equals("main")? "\t\t.entrypoint\n": "";
            var exit = "\t\tldc.i4 0\n";
            exit += node.AnchorToken.Value.Equals("main")? "\t\tcall void class ['mscorlib']'System'.'Environment'::'Exit'(int32)\n\t\tret\n": "\t\tret\n";
            var parameterList = Visit ((dynamic) node[0]);
            var varDefinition = Visit((dynamic) node[1]);
            var body = Visit((dynamic) node[2]);
            var returnType = node.AnchorToken.Value.Equals("main")? "void": "int32";
            var result =  "\t.method public static "+returnType+ "  '" + node.AnchorToken.Value + "'(" + parameterList+ ") {\n"+ entryPoint+ varDefinition + body + exit + "\n\t}\n";
            Variables.CleanArgs();
            return result;
        }

        public string Visit (VarDefinitionList node) {
            var result = "";
            foreach(var child in node) {
                result+= Visit((dynamic) child);
            }
            return result;
        }

        public string Visit (ParameterList node) {
            var result = "";
            foreach (var n in node) {
                var id = n.AnchorToken.Value;
                Variables.Add(new Variable(id, "!arg"));
                result += "int32 '" + id + "', ";
            }
            if (result.Contains(",")){
                result =  result.Substring(0, result.LastIndexOf(","));
            }
            return result;
        }

        public string Visit (StatementList node) {
            var result = "";
            foreach(var child in node){
                result += Visit((dynamic) child);
            }
            return result;
        } 

        public string Visit (If node) {
            var result = "";
            result += node[0].GetType() == typeof(Identifier)? loadVariable(node[0].AnchorToken.Value): Visit((dynamic) node[0]);
            var labelElseIf = GenerateLabel();
            var labelContinue = GenerateLabel();
            var labelElse = GenerateLabel();
            result += "\t\tldc.i4 42\n\t\tbne.un " + labelElseIf + "\n";
            result += Visit((dynamic) node[1]);
            result += "\t\tbr " + labelContinue + "\n";
            result += "\t\t" + labelElseIf + ":\n";
            var elseIfStmt = Visit((ElseIfList) node[2], labelElse, labelContinue);
            result += elseIfStmt.Length == 0? "\t\tnop\n": elseIfStmt;
            result += "\t\t" + labelElse + ":\n";
            var elseStmt = Visit((Else) node[3], labelContinue);
            result += elseStmt.Length == 0? "\t\tnop\n": elseStmt;
            result += "\t\t"+labelContinue + ":\n";
            return result;
        }

        public string Visit (NotEqual node) {
            var result = "\t\tldc.i4 42\n";
            result += Visit((dynamic) node[0]);
            result += Visit((dynamic) node[1]);
            var labelSuccess = GenerateLabel();
            result += "\t\tbne.un "+ labelSuccess + "\n";
            result += "\t\tpop\n";
            result += "\t\tldc.i4 0\n";
            result += "\t\t" + labelSuccess + ":\n";
            return result;
        }

        public string Visit (Assignment node) {
            return Visit((dynamic)node[0]) + "\t\t" + getVariableType(node.AnchorToken.Value)+ "\n";
        }

        public string getVariableType(string lexeme){
            if(Variables.Contains(lexeme, Scope)){
                return "stloc '" + lexeme + "'";
            } else if(Variables.Contains(lexeme, "!arg")){
                return "starg '" + lexeme + "'";
            }
            return "stsfld int32 'DeepLingoCompiler'::'" + lexeme + "'";
        }

        public string Visit (Plus node) {
            return Visit((dynamic)node[0]) + Visit((dynamic)node[1]) + "\t\tadd\n";
        }

        public string Visit (Minus node) {
            if(node.ChildrenSize() <= 1){
                return Visit((dynamic)node[0]) + "\t\tneg\n";
            }
            return Visit((dynamic)node[0]) + Visit((dynamic)node[1]) + "\t\tsub.ovf\n";
        }

        public string Visit (Greater node) { 
            var result = "";
            result += "\t\tldc.i4 42\n";
            result += Visit((dynamic) node[0]);
            result += Visit((dynamic) node[1]);
            var labelSuccess = GenerateLabel();
            result += "\t\tbgt "+ labelSuccess + "\n";
            result += "\t\tpop\n";
            result += "\t\tldc.i4 0\n";
            result += "\t\t" + labelSuccess + ":\n";
            return result;
        }

        public string Visit (GreaterEqual node) {
            var result = "";
            result += "\t\tldc.i4 42\n";
            result += Visit((dynamic) node[0]);
            result += Visit((dynamic) node[1]);
            var labelSuccess = GenerateLabel();
            result += "\t\tbge "+ labelSuccess + "\n";
            result += "\t\tpop\n";
            result += "\t\tldc.i4 0\n";
            result += "\t\t" + labelSuccess + ":\n";
            return result;
        }

        public string Visit(Equal node) {
            var result = "";
            result += "\t\tldc.i4 42\n";
            result += Visit((dynamic) node[0]);
            result += Visit((dynamic) node[1]);
            var labelSuccess = GenerateLabel();
            result += "\t\tbeq "+ labelSuccess + "\n";
            result += "\t\tpop\n";
            result += "\t\tldc.i4 0\n";
            result += "\t\t" + labelSuccess + ":\n";
            return result;
        }

        public string Visit(Neg node) {
            var result = "";
            result += "\t\tldc.i4 42\n";
            result += Visit((dynamic) node[0]);
            var labelSuccess = GenerateLabel();
            result += "\t\tldc.i4 42\n";
            result += "\t\tbne.un "+ labelSuccess + "\n";
            result += "\t\tpop\n";
            result += "\t\tldc.i4 0\n";
            result += "\t\t" + labelSuccess + ":";
            return result;
        }

        public string Visit (Less node) {
            var result = "";
            result += "\t\tldc.i4 42\n";
            result += Visit((dynamic) node[0]);
            result += Visit((dynamic) node[1]);
            var labelSuccess = GenerateLabel();
            result += "\t\tblt "+ labelSuccess + "\n";
            result += "\t\tpop\n";
            result += "\t\tldc.i4 0\n";
            result += "\t\t" + labelSuccess + ":\n";
            return result;
        }

        public string Visit (LessEqual node) {
            var result = "";
            result += "\t\tldc.i4 42\n";
            result += Visit((dynamic) node[0]);
            result += Visit((dynamic) node[1]);
            var labelSuccess = GenerateLabel();
            result += "\t\tble "+ labelSuccess + "\n";
            result += "\t\tpop\n";
            result += "\t\tldc.i4 0\n";
            result += "\t\t" + labelSuccess + ":\n";
            return result;
        }

        public string Visit (And node) {
            var result = "";
            result += "\t\tldc.i4 0\n";
            result += Visit((dynamic)node[0]);
            var label1 = GenerateLabel();
            result += "\t\tldc.i4 42\n";
            result += "\t\tbne.un " + label1 + "\n";
            result += Visit((dynamic)node[1]);
            result += "\t\tldc.i4 42\n";
            result += "\t\tbne.un " + label1 + "\n";
            result += "\t\tpop\n";
            result += "\t\tldc.i4 42\n";
            result += "\t\t" + label1 + ":\n";
            return result;
        }

        public string Visit (Or node) {
            var result = "";
            result += "\t\tldc.i4 42\n";
            result += Visit((dynamic)node[0]);
            var label1 = GenerateLabel();
            result += "\t\tldc.i4 42\n";
            result += "\t\tbeq " + label1 + "\n";
            result += Visit((dynamic)node[1]);
            result += "\t\tldc.i4 42\n";
            result += "\t\tbeq " + label1 + "\n";
            result += "\t\tpop\n";
            result += "\t\tldc.i4 0\n";
            result += "\t\t" + label1 + ":\n";
            return result;
        }

        public string Visit (Return node) {
            return Visit((dynamic) node[0]) + "\t\tret\n";
        }

        public string Visit (Array node) { 
            var result = "\t\tldc.i4 0\n";
            result += "\t\tcall int32 class ['deeplingolib']'DeepLingo'.'Utils'::'New'(int32)\n";
            foreach(var child in node[0]){
                result += "\t\tdup\n";
                result += Visit((dynamic)child);
                result += "\t\tcall int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Add'(int32, int32)\n\t\tpop\n";
            }
            return result;
        }

        public void Visit (ExpressionList node) {
            VisitChildren(node);
        }

        public string loadVariable(string lexeme){
            if(Variables.Contains(lexeme, Scope)){
                return "\t\tldloc '" + lexeme + "'\n";
            } else if(Variables.Contains(lexeme, "!arg")){
                return "\t\tldarg '" + lexeme + "'\n";
            }
            return "\t\tldsfld int32 'DeepLingoCompiler'::'" + lexeme + "'\n";
        }

        public string Visit (FunctionCall node) {
            var result = "";
            foreach(var child in node[0]) {
                if(child.GetType() == typeof(Identifier)){
                    result += loadVariable(child.AnchorToken.Value);
                    continue;
                }
                result += Visit((dynamic)child);
            }
            result += getFunctionCall(node);
            return result;
        }

        public string getFunctionCall(FunctionCall node){
            var result = "";
            var id = node.AnchorToken.Value;
            var pop = "";
            if(Functions.Contains(node.AnchorToken.Value)) {
                pop = Functions[id] == 1 ? "\t\tpop\n":"";
                id = id.Remove(0,1).Insert(0, Char.ToUpper(id[0]).ToString());
                result += "\t\tcall int32 class ['deeplingolib']'DeepLingo'.'Utils'::'" + id + "'(";
            } else {
                pop = node.Parent != null && node.Parent.GetType() == typeof(StatementList) ? "\t\tpop\n":"";
                result += "\t\tcall int32 class ['compiler'] 'DeepLingoCompiler'::'" + id + "'(";
            }
            for(var i = 0; i< node[0].ChildrenSize(); i++){
                result += "int32 ,";
            }
            if (result.Contains(",")){
                result =  result.Substring(0, result.LastIndexOf(","));
            }
            result+= ")\n" + pop;
            return result;
        }

        public string Visit (ElseIfList node, string labelElse, string labelContinue) {
            var result = "";
            foreach(var child in node){
                result += Visit((ElseIf) child, labelContinue);
            }
            result += "\t\tbr " + labelElse + "\n";
            return result;
        }

        public string Visit (ElseIf node, string labelContinue) {
            var result = "";
            result += node[0].GetType() == typeof(Identifier)? loadVariable(node[0].AnchorToken.Value): Visit((dynamic) node[0]);
            var label1 = GenerateLabel();
            result += "\t\tldc.i4 42\n\t\tbne.un " + label1 +"\n";
            result += Visit((dynamic) node[1]);
            result += "\t\tbr " + labelContinue + "\n";
            result += "\t\t" + label1 + ":\n";
            return result;
        }

        public string Visit (Else node, string labelContinue) {
            var result = "";
            if(node.ChildrenSize() > 0){
                result += Visit((dynamic) node[0]);
                result += "\t\tbr " + labelContinue + "\n";
            }
            return result;
        }

        public string Visit (IntegerLiteral node) {
            try {
                var num = Convert.ToInt32(node.AnchorToken.Value);
                return "\t\tldc.i4 " + num + "\n";
            } catch(Exception e){
                Console.WriteLine(e);
                throw new SemanticError("Invalid integer literal", node.AnchorToken);
            }
        }

        public string Visit (CharacterLiteral node) {
            var str = node.AnchorToken.Value;
            str = str.Substring(1, str.Length-2);
            var character = ' ';
            if(str.Contains(@"\u")){
                str = str.Replace(@"\u", "");
                character = Convert.ToChar(Convert.ToInt32(str, 16));
            } else if(codePoints.Keys.Contains(str)){
                character = Convert.ToChar(codePoints[str]);
            }else {
                character = Char.Parse(str);
            }
            var unicode = (int) character;
            var result = "\t\tldc.i4 "+ unicode + "\n";
            return result;
        }

        public string Visit (StringLiteral node) {
            var result = "\t\tldc.i4 0\n";
            result += "\t\tcall int32 class ['deeplingolib']'DeepLingo'.'Utils'::'New'(int32)\n";
            var literal = node.AnchorToken.Value;
            var str = literal.Substring(1, literal.Length-2);
            str = ProcessCharString(str);
            foreach(var tok in codePoints){
                str = str.Replace(tok.Key, Convert.ToChar(tok.Value).ToString());
            }
            var charArray = str.ToCharArray();
            foreach(var character in charArray){
                result += "\t\tdup\n";
                var unicode = (int) character;
                result += "\t\tldc.i4 "+ unicode + "\n";
                result += "\t\tcall int32 class ['deeplingolib']'DeepLingo'.'Utils'::'Add'(int32, int32)\n\t\tpop\n";
            }
            return result;
        }

        public string ProcessCharString(string str) {
            var rem = str;
            var result = str;
            var hex = "";
            while(rem.Contains(@"\u")){
                hex = rem.Substring(rem.IndexOf(@"\u") + 2, 6);
                rem = rem.Substring(rem.IndexOf(@"\u") + 8);
                var character = Convert.ToChar(Convert.ToInt32(hex, 16)).ToString();
                result = result.Replace(@"\u"+ hex, character);
            }
            return result;
        }

        public string Visit (Mul node) {
            return Visit((dynamic)node[0]) + Visit((dynamic)node[1]) + "\t\tmul\n";
        }

        public string Visit (Div node) {
            return Visit((dynamic)node[0]) + Visit((dynamic)node[1]) + "\t\tdiv\n";
        }

        public string Visit (Mod node) {
            return Visit((dynamic)node[0]) + Visit((dynamic)node[1]) + "\t\trem\n";
        }

        public string Visit (Identifier node) {
            var lexeme = node.AnchorToken.Value;
            if(Variables.Contains(lexeme, Scope)){
                return "\t\tldloc '" + lexeme + "'\n";
            } else if(Variables.Contains(lexeme, "!arg")){
                return "\t\tldarg '" + lexeme + "'\n";
            }
            return "\t\tldsfld int32 'DeepLingoCompiler'::'" + lexeme + "'\n";
        }

        public string Visit (Break node) {
            var tok = Scope.Split('-');
            var labelContinue = tok[tok.Length-1];
            return "\t\tbr " + labelContinue + "\n";
        }

        public string Visit (Loop node) { 
            var loopLabel = GenerateLabel();
            var labelContinue = GenerateLabel();
            Scope += "-" + labelContinue;
            var result = "\t\t"+loopLabel +":\n";
            foreach(var child in node[0]) {
                result += Visit((dynamic) child);
            }
            result += "\t\tbr "+ loopLabel +"\n";
            result += "\t\t"+labelContinue +":\n";
            var index = Scope.LastIndexOf('-');
            Scope = Scope.Substring(0, index);
            return result;
        }

        public string Visit (Increment node) {
            var id = node.AnchorToken.Value;
            var result = "\t\tldc.i4 1\n";
            result += loadVariable(id);
            result += "\t\tadd\n";
            result += "\t\t" + getVariableType(id) + "\n";
            return result;
        }


        public string Visit (Decrement node) {
            var id = node.AnchorToken.Value;
            var result = loadVariable(id);
            result += "\t\tldc.i4 1\n";
            result += "\t\tsub.ovf\n";
            result += "\t\t" + getVariableType(id) + "\n";
            return result;
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