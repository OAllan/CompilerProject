/*
  Buttercup compiler - Parent Node class for the AST (Abstract Syntax Tree).
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

using System; 
using System.Collections.Generic;
using System.Text;

namespace DeepLingo {

    public class Node: IEnumerable<Node> {

        IList<Node> children = new List<Node>();

        public Node Parent{
            get;
            private set;
        }

        public Node this[int index] {
            get {
                return children[index];
            }
        }

        public Token AnchorToken { get; set; }

        public Node SetParent(Node Parent) {
            this.Parent = Parent;
            return this;
        }

        public void Add(Node node) {
            node.SetParent(this);
            children.Add(node);
        }

        public IEnumerator<Node> GetEnumerator() {
            return children.GetEnumerator();
        }

        System.Collections.IEnumerator
                System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return String.Format("{0} {1} {2}", GetType().Name, AnchorToken, Parent != null? Parent.GetType().Name:"");                                 
        }

        public string ToStringTree() {
            var sb = new StringBuilder();
            TreeTraversal(this, "", sb);
            return sb.ToString();
        }

        static void TreeTraversal(Node node, string indent, StringBuilder sb) {
            sb.Append(indent);
            sb.Append(node);
            sb.Append('\n');
            foreach (var child in node.children) {
                TreeTraversal(child, indent + "  ", sb);
            }
        }

        public int ChildrenSize() {
            return children.Count;
        }
    }
}
