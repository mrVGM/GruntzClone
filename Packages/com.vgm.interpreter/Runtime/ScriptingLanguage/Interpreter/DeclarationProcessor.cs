using System;
using ScriptingLanguage.Parser;

namespace ScriptingLanguage.Interpreter
{
    public class DeclarationProcessor : IProgramNodeProcessor
    {
        IProgramNodeProcessor ExpressionProcessor = new ExpressionNodeProcessor();
        public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
        {
            if (programNode.Token.Name != "Declaration") 
            {
                throw new NotSupportedException();
            }

            if (programNode.MatchChildren("let", "Name", "=", "Expression", ";"))
            {
                string varName = programNode.Children[1].Token.Data as string;

                if (scope.HasVariable(varName as string, true)) 
                {
                    throw new Exception("Variable already declared!");
                }

                object val = null;
                NodeProcessor.ExecuteProgramNodeProcessor(ExpressionProcessor, programNode.Children[3], scope, ref val);
                scope.AddVariable(varName as string, val);
                return true;
            }

            throw new NotImplementedException();
        }
    }
}
