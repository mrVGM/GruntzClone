using System;
using System.Collections.Generic;
using System.Linq;
using ScriptingLanguage.Parser;

namespace ScriptingLanguage.Interpreter
{
    public class FunctionDeclarationProcessor : IProgramNodeProcessor
    {
        public class FunctionScopeAndBlock 
        {
            public IEnumerable<string> ScopeVariables;
            public ProgramNode Block;
        }

        public class ParametersProcessor : IProgramNodeProcessor
        {
            public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
            {
                if (programNode.Token.Name != "Parameters") 
                {
                    throw new NotSupportedException();
                }
                if (programNode.MatchChildren("Name")) 
                {
                    value = new string[] { programNode.Children[0].Token.Data as string };
                    return null;
                }
                if (programNode.MatchChildren("Parameters", ",", "Name")) 
                {
                    object tmp = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(this, programNode.Children[0], scope, ref tmp);
                    var paramsList = (tmp as IEnumerable<string>).ToList();
                    paramsList.Add(programNode.Children[2].Token.Data as string);
                    value = paramsList;
                    return null;
                }
                throw new NotImplementedException();
            }
        }

        IProgramNodeProcessor Parameters = new ParametersProcessor();
        public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
        {
            if (programNode.Token.Name != "FunctionDeclaration") 
            {
                throw new NotSupportedException();
            }

            if (programNode.MatchChildren("function", "(", ")", "Block")) 
            {
                value = new FunctionScopeAndBlock { ScopeVariables = new string[] { }, Block = programNode.Children[3] };
                return null;
            }

            if (programNode.MatchChildren("function", "(", "Parameters", ")", "Block")) 
            {
                object tmp = null;
                NodeProcessor.ExecuteProgramNodeProcessor(Parameters, programNode.Children[2], scope, ref tmp);
                var parameters = tmp as IEnumerable<string>;
                value = new FunctionScopeAndBlock { ScopeVariables = parameters, Block = programNode.Children[4] };
                return null;
            }

            throw new NotImplementedException();
        }
    }
}
