using System;
using System.Linq;
using System.Reflection;
using ScriptingLanguage.Parser;

namespace ScriptingLanguage.Interpreter
{
    public class OperationProcessor : IProgramNodeProcessor
    {
        public class BreakOperation { }
        public class ReturnOperation 
        {
            public bool ReturnExpression = true;
        }

        static IProgramNodeProcessor ValueProcessor = new ValueNodeProcessor();
        static IProgramNodeProcessor ExpressionProcessor = new ExpressionNodeProcessor();
        static IProgramNodeProcessor BooleanExpressionProcessor = new BooleanExpressionProcessor();
        static IProgramNodeProcessor NameProcessor = new NameProcessor();
        static IProgramNodeProcessor AssignProcessor = new AssignmentProcessor();
        static IProgramNodeProcessor DeclarationProcessor = new DeclarationProcessor();
        static IProgramNodeProcessor IfProcessor = new IfProcessor();
        static IProgramNodeProcessor WhileProcessor = new WhileProcessor();

        class SetValueProcessor : IProgramNodeProcessor
        {
            object ValueToSet;
            public SetValueProcessor(object valueToSet) 
            {
                ValueToSet = valueToSet;
            }
            public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
            {
                if (programNode.Token.Name != "Value") 
                {
                    throw new NotSupportedException("Wrong node supported");
                }

                if (programNode.MatchChildren("Name")) 
                {
                    var child = programNode.Children[0];
                    scope.SetVariable(child.Token.Data as string, ValueToSet);
                    return null;
                }

                if (programNode.MatchChildren("Value", ".", "Name"))
                {
                    object val = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(ValueProcessor, programNode.Children[0], scope, ref val);

                    string propertyName = programNode.Children[2].Token.Data as string;

                    var genericObject = val as GenericObject;
                    if (genericObject != null)
                    {
                        var objectContainer = genericObject.GetPropoerty(propertyName);
                        objectContainer.ObjectValue = ValueToSet;
                        return true;
                    }

                    var staticMethodPath = val as StaticMethodPath;
                    if (staticMethodPath != null)
                    {
                        staticMethodPath.Path += "." + propertyName;
                        staticMethodPath.SetStaticProperty(ValueToSet);
                        return true;
                    }

                    Utils.SetProperty(val, propertyName, ValueToSet);
                    return true;
                }

                if (programNode.MatchChildren("Value", "[", "Expression", "]")) 
                {
                    object val = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(ValueProcessor, programNode.Children[0], scope, ref val);

                    object expr = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(ExpressionProcessor, programNode.Children[2], scope, ref expr);

                    var staticMethodPath = val as StaticMethodPath;
                    if (staticMethodPath != null) 
                    {
                        val = staticMethodPath.GetStaticProperty();
                        var number = expr as Number;
                        if (number != null)
                        {
                            expr = number.GetNumber(typeof(int));
                        }
                    }

                    var type = val.GetType();

                    var genericArguments = type.GetGenericArguments();
                    if (genericArguments.Count() == 0)
                    {
                        var valueToSet = ValueToSet;
                        var method = type.GetMethod("Set");
                        var p = method.GetParameters().FirstOrDefault();
                        if (Utils.IsNumber(p.ParameterType) && ValueToSet is Number) 
                        {
                            valueToSet = (ValueToSet as Number).GetNumber(p.ParameterType);
                        }
                        method.Invoke(val, new object[] { Convert.ToInt32(expr), valueToSet });
                        return true;
                    }

                    if (genericArguments.Count() == 1) 
                    {
                        var valueToSet = ValueToSet;
                        var method = type.GetMethod("set_Item");
                        var p = method.GetParameters().FirstOrDefault();
                        if (Utils.IsNumber(p.ParameterType) && ValueToSet is Number)
                        {
                            valueToSet = (ValueToSet as Number).GetNumber(p.ParameterType);
                        }
                        method.Invoke(val, new object[] { Convert.ToInt32(expr), valueToSet });
                        return true;
                    }

                    if (genericArguments.Count() == 2)
                    {
                        var key = expr;
                        if (typeof(int).IsAssignableFrom(genericArguments[0]))
                        {
                            key = Convert.ToInt32(expr);
                        }

                        var valueToSet = ValueToSet;
                        var method = type.GetMethod("set_Item");
                        var p = method.GetParameters().FirstOrDefault();
                        if (Utils.IsNumber(p.ParameterType) && ValueToSet is Number)
                        {
                            valueToSet = (ValueToSet as Number).GetNumber(p.ParameterType);
                        }
                        method.Invoke(val, new object[] { key, valueToSet });
                        return true;
                    }

                    throw new NotImplementedException();
                }
                throw new NotImplementedException();
            }
        }

        class AssignmentProcessor : IProgramNodeProcessor
        {
            public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
            {
                if (programNode.Token.Name != "Assignment")
                {
                    throw new Exception("Wrong node supported");
                }

                if (programNode.MatchChildren("Value", "=", "Expression", ";")) 
                {
                    object val1 = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(ValueProcessor, programNode.Children[0], scope, ref val1);

                    object val2 = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(ExpressionProcessor, programNode.Children[2], scope, ref val2);
                    if (val1 is GenericObject.ObjectContainer)
                    {
                        var objectContainer = val1 as GenericObject.ObjectContainer;
                        objectContainer.ObjectValue = val2;
                        return true;
                    }

                    var setValueProcessor = new SetValueProcessor(val2);
                    NodeProcessor.ExecuteProgramNodeProcessor(setValueProcessor, programNode.Children[0], scope, ref value);
                    return true;
                }

                if (programNode.MatchChildren("Value", "=", "BooleanExpression", ";"))
                {
                    object val1 = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(ValueProcessor, programNode.Children[0], scope, ref val1);

                    object val2 = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(BooleanExpressionProcessor, programNode.Children[2], scope, ref val2);
                    if (val1 is GenericObject.ObjectContainer)
                    {
                        var objectContainer = val1 as GenericObject.ObjectContainer;
                        objectContainer.ObjectValue = val2;
                        return true;
                    }

                    var setValueProcessor = new SetValueProcessor(val2);
                    NodeProcessor.ExecuteProgramNodeProcessor(setValueProcessor, programNode.Children[0], scope, ref value);
                    return true;
                }

                throw new NotImplementedException();
            }
        }
        public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
        {
            if (programNode.Token.Name != "Operation")
            {
                throw new Exception("Wrong node supported!");
            }
            if (programNode.MatchChildren("Value", ";")) 
            {
                var res = NodeProcessor.ExecuteProgramNodeProcessor(ValueProcessor, programNode.Children[0], scope, ref value);
                var staticMethodPath = value as StaticMethodPath;
                if (staticMethodPath != null) 
                {
                    value = staticMethodPath.GetStaticProperty();
                }
                return res;
            }

            if (programNode.MatchChildren("Assignment"))
            {
                return NodeProcessor.ExecuteProgramNodeProcessor(AssignProcessor, programNode.Children[0], scope, ref value);
            }

            if (programNode.MatchChildren("Declaration")) 
            {
                return NodeProcessor.ExecuteProgramNodeProcessor(DeclarationProcessor, programNode.Children[0], scope, ref value);
            }

            if (programNode.MatchChildren("break", ";")) 
            {
                return new BreakOperation();
            }

            if (programNode.MatchChildren("IfStatement")) 
            {
                return NodeProcessor.ExecuteProgramNodeProcessor(IfProcessor, programNode.Children[0], scope, ref value);
            }
            if (programNode.MatchChildren("WhileStatement"))
            {
                return NodeProcessor.ExecuteProgramNodeProcessor(WhileProcessor, programNode.Children[0], scope, ref value);
            }
            if (programNode.MatchChildren("return", ";"))
            {
                value = null;
                return new ReturnOperation { ReturnExpression = false };
            }
            if (programNode.MatchChildren("return", "Expression", ";"))
            {
                NodeProcessor.ExecuteProgramNodeProcessor(ExpressionProcessor, programNode.Children[1], scope, ref value);
                return new ReturnOperation();
            }
            if (programNode.MatchChildren("return", "BooleanExpression", ";"))
            {
                NodeProcessor.ExecuteProgramNodeProcessor(BooleanExpressionProcessor, programNode.Children[1], scope, ref value);
                return new ReturnOperation();
            }

            throw new NotImplementedException();
        }
    }
}
