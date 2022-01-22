using System;
using ScriptingLanguage.Parser;

namespace ScriptingLanguage.Interpreter
{
    class IfProcessor : IProgramNodeProcessor
    {
        IProgramNodeProcessor BlockProcessor = new BlockProcessor();
        IProgramNodeProcessor BooleanExpressionProcessor = new BooleanExpressionProcessor();
        public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
        {
            if (programNode.Token.Name != "IfStatement") 
            {
                throw new NotSupportedException();
            }

            if (programNode.MatchChildren("if", "(", "BooleanExpression", ")", "Block")) 
            {
                object expr = null;
                NodeProcessor.ExecuteProgramNodeProcessor(BooleanExpressionProcessor, programNode.Children[2], scope, ref expr);
                if ((bool)expr) 
                {
                    var ifScope = new Scope();
                    ifScope.ParentScope = scope;
                    return NodeProcessor.ExecuteProgramNodeProcessor(BlockProcessor, programNode.Children[4], ifScope, ref value);
                }
                return true;
            }

            throw new NotImplementedException();
        }
    }
}
