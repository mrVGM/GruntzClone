using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScriptingLanguage.Parser;

namespace ScriptingLanguage.Interpreter
{
    public class StaticMethodPath
    {
        public string Path = "";
        public object GetStaticProperty() 
        {
            if (string.IsNullOrWhiteSpace(Path)) 
            {
                return null;
            }

            int dotIndex = Path.LastIndexOf('.');
            if (dotIndex < 0) 
            {
                return null;
            }

            string typeName = Path.Substring(0, dotIndex);
            string propertyName = Path.Substring(dotIndex + 1);

            var type = Utils.GetTypeAcrossAssemblies(typeName);
            if (type == null) 
            {
                return null;
            }

            return Utils.GetProperty(null, type, propertyName);
        }

        public void SetStaticProperty(object val) 
        {
            if (string.IsNullOrWhiteSpace(Path))
            {
                return;
            }

            int dotIndex = Path.LastIndexOf('.');
            if (dotIndex < 0)
            {
                return;
            }

            string typeName = Path.Substring(0, dotIndex);
            string propertyName = Path.Substring(dotIndex + 1);

            var type = Utils.GetTypeAcrossAssemblies(typeName);
            if (type == null)
            {
                return;
            }

            Utils.SetProperty(null, type, propertyName, val);
        }
    }
    class ValueNodeProcessor : IProgramNodeProcessor
    {
        public IProgramNodeProcessor NumberProcessor = new NumberNodeProcessor();
        public IProgramNodeProcessor StringProcessor = new StringNodeProcessor();
        public IProgramNodeProcessor ExpressionProcessor = new ExpressionNodeProcessor();
        public IProgramNodeProcessor NameProcessor = new NameProcessor();
        public IProgramNodeProcessor FunctionCall = new FunctionCallProcessor();
        public IProgramNodeProcessor FunctionDeclaration = new FunctionDeclarationProcessor();

        public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
        {
            var token = programNode.Token;
            if (token.Name != "Value")
            {
                return false;
            }

            var childNode = programNode.Children.First();
            var nodeNames = programNode.Children.Select(x => x.Token.Name).ToArray();
            if (programNode.Children.Count == 1)
            {
                switch (childNode.Token.Name)
                {
                    case "Number":
                        return NodeProcessor.ExecuteProgramNodeProcessor(NumberProcessor, childNode, scope, ref value);
                    case "String":
                        return NodeProcessor.ExecuteProgramNodeProcessor(StringProcessor, childNode, scope, ref value);
                    case "Name":
                        {
                            var name = childNode.Token.Data as string;
                            if (!scope.HasVariable(name))
                            {
                                value = new StaticMethodPath { Path = name };
                                return null;
                            }
                            return NodeProcessor.ExecuteProgramNodeProcessor(NameProcessor, childNode, scope, ref value);
                        }
                    case "null":
                        value = null;
                        return null;
                    case "true":
                        value = true;
                        return null;
                    case "false":
                        value = false;
                        return null;
                    case "FunctionCall":
                        {
                            object tmp = null;
                            NodeProcessor.ExecuteProgramNodeProcessor(FunctionCall, childNode, scope, ref tmp);
                            var funcSettings = tmp as FunctionCallProcessor.FunctionCallSettings;

                            var func = scope.GetVariable(funcSettings.FunctionName);
                            var iFunc = func as IFunction;
                            if (iFunc != null)
                            {
                                var functionScope = iFunc.ScopeTemplate;
                                foreach (var param in iFunc.ParameterNames)
                                {
                                    functionScope.SetVariable(param, null);
                                }

                                if (iFunc.ParameterNames.Length > 0)
                                {
                                    int index = 0;
                                    foreach (var arg in funcSettings.Arguments)
                                    {
                                        functionScope.SetVariable(iFunc.ParameterNames[index++], arg);

                                        if (index == iFunc.ParameterNames.Length)
                                        {
                                            break;
                                        }
                                    }
                                }
                                value = iFunc.Execute(functionScope);
                                return null;
                            }
                            throw new NotImplementedException();
                        }
                    case "FunctionDeclaration":
                        {
                            object tmp = null;
                            NodeProcessor.ExecuteProgramNodeProcessor(FunctionDeclaration, childNode, scope, ref tmp);
                            var funcScopeAndBlock = tmp as FunctionDeclarationProcessor.FunctionScopeAndBlock;

                            var func = new Function(scope);
                            func.Block = funcScopeAndBlock.Block;
                            func.ParameterNames = funcScopeAndBlock.ScopeVariables.ToArray();

                            value = func;
                            return null;
                        }
                    default:
                        return NodeProcessor.ExecuteProgramNodeProcessor(DummyNodeProcessor.DummyProcessor, childNode, scope, ref value);
                }
            }

            if (programNode.MatchChildren("Value", ".", "Name"))
            {
                object val1 = null;
                NodeProcessor.ExecuteProgramNodeProcessor(this, programNode.Children[0], scope, ref val1);
                string propertyName = programNode.Children[2].Token.Data as string;

                var staticMethodPath = val1 as StaticMethodPath;
                if (staticMethodPath != null) 
                {
                    staticMethodPath.Path += $".{propertyName}";
                    value = staticMethodPath;
                    return null;
                }

                var genericObject = val1 as GenericObject;
                if (genericObject != null)
                {
                    value = genericObject.GetPropoerty(propertyName);
                    return true;
                }

                value = Utils.GetProperty(val1, propertyName);
                return true;
            }

            if (programNode.MatchChildren("{", "}")) 
            {
                value = new GenericObject();
                return true;
            }

            if (programNode.MatchChildren("[", "]"))
            {
                value = new List<object>();
                return true;
            }

            if (programNode.MatchChildren("Value", "[", "Expression", "]"))
            {
                object val1 = null;
                NodeProcessor.ExecuteProgramNodeProcessor(this, programNode.Children[0], scope, ref val1);
                object val2 = null;
                NodeProcessor.ExecuteProgramNodeProcessor(ExpressionProcessor, programNode.Children[2], scope, ref val2);

                var type = val1.GetType();
                var genericArguments = type.GetGenericArguments();

                var staticMethodPath = val1 as StaticMethodPath;
                if (staticMethodPath != null) 
                {
                    val1 = staticMethodPath.GetStaticProperty();
                }

                if (val1 is IEnumerable) 
                {
                    if (genericArguments.Length < 2)
                    {
                        var tmpList = new List<object>();
                        foreach (var obj in val1 as IEnumerable) 
                        {
                            tmpList.Add(obj);
                        }
                        value = tmpList[(int)(val2 as Number).GetNumber(typeof(int))];
                        return null;
                    }
                    if (genericArguments.Length == 2) 
                    {
                        object key = val2;
                        if (val2 is Number) 
                        {
                            key = (val2 as Number).GetNumber(genericArguments[0]);
                        }
                        var method = type.GetMethod("TryGetValue");
                        object res = null;
                        object[] par = { key, res };
                        method.Invoke(val1, par);
                        value = par[1];
                        return true;
                    }
                }
            }

            if (programNode.MatchChildren("Value", ".", "FunctionCall")) 
            {
                object obj = null;
                NodeProcessor.ExecuteProgramNodeProcessor(this, programNode.Children[0], scope, ref obj);

                object tmp = null;
                NodeProcessor.ExecuteProgramNodeProcessor(FunctionCall, programNode.Children[2], scope, ref tmp);
                var settings = tmp as FunctionCallProcessor.FunctionCallSettings;

                var genericObject = obj as GenericObject;
                if (genericObject != null)
                {
                    var property = genericObject.GetPropoerty(settings.FunctionName);
                    var iFunc = property.ObjectValue as IFunction;
                    if (iFunc != null)
                    {
                        var functionScope = iFunc.ScopeTemplate;
                        foreach (var param in iFunc.ParameterNames)
                        {
                            functionScope.SetVariable(param, null);
                        }

                        if (iFunc.ParameterNames.Length > 0)
                        {
                            int index = 0;
                            foreach (var arg in settings.Arguments)
                            {
                                functionScope.SetVariable(iFunc.ParameterNames[index++], arg);

                                if (index == iFunc.ParameterNames.Length)
                                {
                                    break;
                                }
                            }
                        }
                        value = iFunc.Execute(functionScope);
                        return null;
                    }
                    throw new NotImplementedException();
                }

                var staticMethodPath = obj as StaticMethodPath;
                Type typeForSearchMethodIn = obj.GetType();
                object objectToCallMethodOn = obj;
                if (staticMethodPath != null) 
                {
                    typeForSearchMethodIn = Utils.GetTypeAcrossAssemblies(staticMethodPath.Path);
                    objectToCallMethodOn = null;
                }
                
                var method = GetMethod(typeForSearchMethodIn, settings);
                var args = settings.Arguments.ToArray();
                var paramTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();

                for (int i = 0; i < args.Length; ++i) 
                {
                    Utils.GetArgumentFor(paramTypes[i], args[i], out args[i]);
                }

                value = method.Invoke(objectToCallMethodOn, args);
                return null;
                
            }

            if (programNode.MatchChildren("-", "Number"))
            {
                object tmp = null;
                NodeProcessor.ExecuteProgramNodeProcessor(NumberProcessor, programNode.Children[1], scope, ref tmp);
                float num = (float)tmp;
                value = -num;
                return null;
            }

            throw new NotImplementedException();
        }

        MethodInfo GetMethod(Type type, FunctionCallProcessor.FunctionCallSettings settings)
        {
            var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var methods = new List<MethodInfo>();

            var curType = type;
            while (curType != null) 
            {
                methods.AddRange(curType.GetMethods(flags).Where(x => x.Name == settings.FunctionName));
                curType = curType.BaseType;
            }

            if (settings.TemplateParamsNames != null && settings.TemplateParamsNames.Any()) 
            {
                var genTypes = settings.TemplateParamsNames.Select(x => Utils.GetTypeAcrossAssemblies(x)).ToArray();
                methods = methods.Where(x => x.GetGenericArguments().Length == genTypes.Count())
                                 .Select(x => x.MakeGenericMethod(genTypes)).ToList();
            }

            if (!methods.Any()) 
            {
                return null;
            }

            var args = settings.Arguments.ToArray();
            settings.Arguments = args;

            bool methodPredicate(MethodInfo method)
            {
                var methodParameters = method.GetParameters();
                if (methodParameters.Length != settings.Arguments.Count())
                {
                    return false;
                }

                for (int i = 0; i < methodParameters.Length; ++i) 
                {
                    var arg = args[i];
                    var paramType = methodParameters[i].ParameterType;

                    object tmp = null;
                    if (!Utils.GetArgumentFor(paramType, arg, out tmp)) 
                    {
                        return false;
                    }
                }
                return true;
            }

            return methods.FirstOrDefault(methodPredicate);
        }
    }
}
