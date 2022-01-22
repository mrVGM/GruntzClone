using System;
using System.Collections.Generic;
using System.Linq;
using ScriptingLanguage.Parser;

namespace ScriptingLanguage.Interpreter
{
    class FunctionCallProcessor : IProgramNodeProcessor
    {
        public class FunctionCallSettings 
        {
            public string FunctionName;
            public IEnumerable<string> TemplateParamsNames;
            public IEnumerable<object> Arguments = new List<object>();
        }

        private class TemplateArgsProcessor : IProgramNodeProcessor
        {
            public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
            {
                string getTemplateArg(ProgramNode node)
                {
                    if (node.Token.Name == "String")
                    {
                        return node.Token.Data as string;
                    }

                    if (node.Token.Name == "Name")
                    {
                        var name = node.Token.Data as string;
                        if (!scope.HasVariable(name))
                        {
                            throw new ArgumentException($"The variable {name} does not exists!");
                        }

                        string template = scope.GetVariable(name) as string;
                        if (string.IsNullOrEmpty(template))
                        {
                            throw new ArgumentException($"The variable {name} should contain a String value!");
                        }

                        return template;
                    }

                    throw new NotSupportedException();
                }

                if (programNode.MatchChildren("String") || programNode.MatchChildren("Name")) 
                {
                    value = new string[] { getTemplateArg(programNode.Children[0]) };
                    return null;
                }
                if (programNode.MatchChildren("TemplateParams", "|", "String") || programNode.MatchChildren("TemplateParams", "|", "Name")) 
                {
                    object tmp = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(this, programNode.Children[0], scope, ref tmp);
                    var templatesSoFar = tmp as IEnumerable<string>;

                    var lastTemplateArg = getTemplateArg(programNode.Children[2]);
                    value = templatesSoFar.Append(lastTemplateArg);
                    return null;
                }
                throw new NotImplementedException();
            }
        }

        public class TemplateProcessor : IProgramNodeProcessor
        {
            private TemplateArgsProcessor TemplateArgsProcessor = new TemplateArgsProcessor();
            public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
            {
                if (programNode.Token.Name != "Template") 
                {
                    throw new NotSupportedException();
                }

                if (programNode.MatchChildren("|", "TemplateParams", "|"))
                {
                    object tmp = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(TemplateArgsProcessor, programNode.Children[1], scope, ref tmp);

                    value = tmp as IEnumerable<string>;
                    return null;
                }

                throw new NotImplementedException();
            }
        }

        public class ArgumentsProcessor : IProgramNodeProcessor
        {
            ExpressionNodeProcessor expressionProcessor = new ExpressionNodeProcessor();
            public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
            {
                if (programNode.Token.Name != "Arguments") 
                {
                    throw new NotSupportedException();
                }

                IEnumerable<object> extractArgs(ProgramNode node)
                {
                    if (node.MatchChildren("Expression")) 
                    {
                        object tmp = null;
                        NodeProcessor.ExecuteProgramNodeProcessor(expressionProcessor, node.Children[0], scope, ref tmp);
                        yield return tmp;
                        yield break;
                    }

                    if (node.MatchChildren("Arguments", ",", "Expression")) 
                    {
                        var arguments = extractArgs(node.Children[0]);
                        foreach (var arg in arguments) 
                        {
                            yield return arg;
                        }

                        object tmp = null;
                        NodeProcessor.ExecuteProgramNodeProcessor(expressionProcessor, node.Children[2], scope, ref tmp);
                        yield return tmp;
                    }
                }

                value = extractArgs(programNode);
                return null;
            }
        }

        IProgramNodeProcessor Template = new TemplateProcessor();
        IProgramNodeProcessor Arguments = new ArgumentsProcessor();

        public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
        {
            if (programNode.Token.Name != "FunctionCall") 
            {
                throw new NotSupportedException();
            }

            if (programNode.MatchChildren("Name", "(", ")")) 
            {
                var name = programNode.Children[0].Token.Data as string;
                value = new FunctionCallSettings { FunctionName = name };
                return null;
            }

            if (programNode.MatchChildren("Name", "Template", "(", ")")) 
            {
                var name = programNode.Children[0].Token.Data as string;

                object template = null;
                NodeProcessor.ExecuteProgramNodeProcessor(Template, programNode.Children[1], scope, ref template);
                value = new FunctionCallSettings { FunctionName = name, TemplateParamsNames = template as IEnumerable<string> };
                return null;
            }

            if (programNode.MatchChildren("Name", "(", "Arguments", ")"))
            {
                var name = programNode.Children[0].Token.Data as string;

                object args = null;
                NodeProcessor.ExecuteProgramNodeProcessor(Arguments, programNode.Children[2], scope, ref args);
                var arguments = args as IEnumerable<object>;
                value = new FunctionCallSettings { FunctionName = name, Arguments = arguments };
                return null;
            }

            if (programNode.MatchChildren("Name", "Template", "(", "Arguments", ")"))
            {
                var name = programNode.Children[0].Token.Data as string;

                object template = null;
                NodeProcessor.ExecuteProgramNodeProcessor(Template, programNode.Children[1], scope, ref template);

                object args = null;
                NodeProcessor.ExecuteProgramNodeProcessor(Arguments, programNode.Children[3], scope, ref args);
                var arguments = args as IEnumerable<object>;
                value = new FunctionCallSettings { FunctionName = name, Arguments = arguments, TemplateParamsNames = template as IEnumerable<string> };
                return null;
            }

            throw new NotImplementedException();
        }
    }
}
