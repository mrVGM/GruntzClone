using System;
using System.Globalization;
using ScriptingLanguage.Parser;

namespace ScriptingLanguage.Interpreter
{
    class NumberNodeProcessor : IProgramNodeProcessor
    {
        public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
        {
            var token = programNode.Token;
            if (token.Name == "Number") 
            {
                string numberString = token.Data as string;
                value = float.Parse(numberString, CultureInfo.InvariantCulture);
                return true;
            }
            return false;
        }
    }
}
