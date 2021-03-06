/*
  Buttercup compiler - Symbol table class.
  Copyright (C) 2013 Ariel Ortiz, ITESM CEM
  
  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.
  
  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  
  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

// This class generates the table where the variables are stored

// Semantic Analysis
// Date: 16-April-2018
// Authors:
//          A01374648 Mario Lagunes Nava 
//          A01375640 Brandon Alain Cruz Ruiz
//          A01376200 Oscar Allan Ruiz Toledo
// File name: SymbolTable.cs

using System;
using System.Text;
using System.Collections.Generic;

namespace DeepLingo{

    public class SymbolTable {

        List<Variable> data = new List<Variable>();

        //-----------------------------------------------------------
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("Symbol Table\n");
            sb.Append("====================\n");
            foreach (var entry in data) {
                sb.Append(String.Format("Variable: {0}. Scope: {1}\n", 
                                        entry.Identifier, 
                                        entry.Scope));
            }
            sb.Append("====================\n");
            return sb.ToString();
        }


        //-----------------------------------------------------------
        public bool Contains(string key, string Scope) {
            var tok = Scope.Split('-');
            Scope = tok[0];
            foreach (var variable in data) {
                if(variable.Identifier.Equals(key) && (variable.Scope.Equals(Scope))) {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(string key) {
            foreach (var variable in data) {
                if(variable.Identifier.Equals(key) && variable.Scope.Equals("global")) {
                    return true;
                }
            }
            return false;
        }

        //-----------------------------------------------------------
        public IEnumerator<Variable> GetEnumerator() {
            return data.GetEnumerator();
        }


        public void Add (Variable variable) {
            data.Add(variable);
        } 


        public void CleanArgs() {
            for (var i = 0; i < data.Count; i++) {
                var variable = data[i];
                if(variable.Scope.Equals("!arg")) {
                    data.RemoveAt(i);
                }
            }
        }
    }
}
