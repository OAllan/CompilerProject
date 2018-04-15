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