using System;
using System.Collections.Generic;
using System.Text;
using ScriptingLanguage.Parser;

namespace ScriptingLanguage.Interpreter
{
    class NameProcessor : IProgramNodeProcessor
    {
        public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
        {
            if (programNode.Token.Name != "Name") 
            {
                throw new Exception("Wrong Node Supported!");
            }

            value = scope.GetVariable(programNode.Token.Data as string);
            return true;
        }
    }
}
