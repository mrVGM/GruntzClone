using System;
using System.Linq;
using ScriptingLanguage.Parser;

namespace ScriptingLanguage.Interpreter
{
    public class ExpressionNodeProcessor : IProgramNodeProcessor
    {
        static IProgramNodeProcessor ValueProcessor = new ValueNodeProcessor();
        static IProgramNodeProcessor ArithmeticExpr = new ArithmeticExpression();
        static IProgramNodeProcessor ProductExpr = new ProductExpression();
        static IProgramNodeProcessor ExpressionProcessor = new ExpressionNodeProcessor();

        class ArithmeticExpression : IProgramNodeProcessor
        {
            public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
            {
                if (programNode.Token.Name != "ArithmethicExpression") 
                {
                    throw new Exception("Unsupported Node Type!");
                }

                if (programNode.MatchChildren("Prod")) 
                {
                    return NodeProcessor.ExecuteProgramNodeProcessor(ProductExpr, programNode.Children[0], scope, ref value);
                }
                if (programNode.MatchChildren("ArithmethicExpression", "+", "Prod")) 
                {
                    object val1 = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(this, programNode.Children[0], scope, ref val1);
                    
                    object val2 = null;
                    NodeProcessor.ExecuteProgramNodeProcessor( ProductExpr, programNode.Children[2], scope, ref val2);

                    if (val1 is Number) 
                    {
                        value = new Number((val1 as Number).DoubleValue + (val2 as Number).DoubleValue);
                        return true;
                    }

                    var type = val1.GetType();
                    var method = type.GetMethod("op_Addition");
                    object[] par = { val1, val2 };
                    value = method.Invoke(null, par);
                    return true;
                }
                if (programNode.MatchChildren("ArithmethicExpression", "-", "Prod"))
                {
                    object val1 = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(this, programNode.Children[0], scope, ref val1);

                    object val2 = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(ProductExpr, programNode.Children[2], scope, ref val2);

                    if (val1 is Number)
                    {
                        value = new Number((val1 as Number).DoubleValue - (val2 as Number).DoubleValue);
                        return true;
                    }

                    var type = val1.GetType();
                    var method = type.GetMethod("op_Subtraction");
                    object[] par = { val1, val2 };
                    value = method.Invoke(null, par);
                    return true;
                }

                throw new NotImplementedException();
            }
        }

        class ProductExpression : IProgramNodeProcessor
        {
            public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
            {
                if (programNode.Token.Name != "Prod")
                {
                    throw new Exception("Unsupported Node Type!");
                }

                if (programNode.MatchChildren("SingleValue"))
                {
                    return NodeProcessor.ExecuteProgramNodeProcessor(ExpressionProcessor, programNode.Children[0], scope, ref value);
                }
                if (programNode.MatchChildren("Prod", "*", "SingleValue"))
                {
                    object val1 = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(this, programNode.Children[0], scope, ref val1);

                    object val2 = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(ExpressionProcessor, programNode.Children[2], scope, ref val2);

                    if (val1 is Number)
                    {
                        value = new Number((val1 as Number).DoubleValue * (val2 as Number).DoubleValue);
                        return true;
                    }

                    var type = val1.GetType();
                    var method = type.GetMethod("op_Multiply");
                    object[] par = { val1, val2 };
                    value = method.Invoke(null, par);
                    return true;
                }
                if (programNode.MatchChildren("Prod", "/", "SingleValue"))
                {
                    object val1 = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(this, programNode.Children[0], scope, ref val1);

                    object val2 = null;
                    NodeProcessor.ExecuteProgramNodeProcessor(ExpressionProcessor, programNode.Children[2], scope, ref val2);

                    if (val1 is Number)
                    {
                        value = new Number((val1 as Number).DoubleValue / (val2 as Number).DoubleValue);
                        return true;
                    }

                    var type = val1.GetType();
                    var method = type.GetMethod("op_Division");
                    object[] par = { val1, val2 };
                    value = method.Invoke(null, par);
                    return true;
                }

                throw new NotImplementedException();
            }
        }

        public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
        {
            var token = programNode.Token;
            var childrenNames = programNode.Children.Select(x => x.Token.Name).ToArray();
            if (token.Name == "Expression")
            {
                return NodeProcessor.ExecuteProgramNodeProcessor(this, programNode.Children[0], scope, ref value);
            }

            if (token.Name == "ArithmethicExpression") 
            {
                return NodeProcessor.ExecuteProgramNodeProcessor(ArithmeticExpr, programNode, scope, ref value);
            }

            if (token.Name == "SingleValue") 
            {
                if (programNode.MatchChildren("Value"))
                {
                    object tmp = null;
                    var res = NodeProcessor.ExecuteProgramNodeProcessor(ValueProcessor, programNode.Children[0], scope, ref tmp);
                    var staticMethodPath = tmp as StaticMethodPath;

                    if (staticMethodPath != null) 
                    {
                        value = staticMethodPath.GetStaticProperty();
                        return res;
                    }

                    value = tmp;
                    var objectContainer = value as GenericObject.ObjectContainer;
                    if (objectContainer != null) {
                        value = objectContainer.ObjectValue;
                    }
                    if (Utils.IsNumber(value)) 
                    {
                        value = Utils.ToNumber(value);
                    }

                    return res;
                }
                if (programNode.MatchChildren("(", "ArithmethicExpression", ")"))
                {
                    return NodeProcessor.ExecuteProgramNodeProcessor(ArithmeticExpr, programNode.Children[1], scope, ref value);
                }

                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }
    }
}
