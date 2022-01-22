using System;
using System.Collections.Generic;
using System.Text;
using ScriptingLanguage.Parser;

namespace ScriptingLanguage.Interpreter
{
    public class DummyNodeProcessor : IProgramNodeProcessor
    {
        public static IProgramNodeProcessor DummyProcessor = new DummyNodeProcessor();
        public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
        {
            throw new NotImplementedException();
        }
    }
}
