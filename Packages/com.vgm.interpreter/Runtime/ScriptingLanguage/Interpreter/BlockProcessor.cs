using System;
using ScriptingLanguage.Parser;

namespace ScriptingLanguage.Interpreter
{
    class BlockProcessor : IProgramNodeProcessor
    {
        IProgramNodeProcessor OperationGroupProcessor = new OperationGroupProcessor();
        public BlockProcessor(OperationGroupProcessor operationGroupProcessor = null) 
        {
            if (operationGroupProcessor != null) 
            {
                OperationGroupProcessor = operationGroupProcessor;
            }
        }
        public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
        {
            if (programNode.Token.Name != "Block") 
            {
                throw new NotSupportedException();
            }

            if (programNode.MatchChildren("{", "OperationGroup", "}")) 
            {
                return NodeProcessor.ExecuteProgramNodeProcessor(OperationGroupProcessor, programNode.Children[1], scope, ref value);
            }
            throw new NotImplementedException();
        }
    }
}
