using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ScriptingLanguage.Parser;

namespace ScriptingLanguage.Interpreter
{
    class ComparisonProcessor : IProgramNodeProcessor
    {
        IProgramNodeProcessor ExpressionProcessor = new ExpressionNodeProcessor();
        public object ProcessNode(ProgramNode programNode, Scope scope, ref object value)
        {
            if (programNode.Token.Name != "Comparison") 
            {
                throw new NotSupportedException();
            }

            var expr1 = programNode.Children[0];
            var expr2 = programNode.Children[2];

            if (expr1.Token.Name != "Expression" || expr2.Token.Name != "Expression") 
            {
                throw new NotSupportedException();
            }

            object e1 = null;
            object e2 = null;
            NodeProcessor.ExecuteProgramNodeProcessor(ExpressionProcessor, expr1, scope, ref e1);
            NodeProcessor.ExecuteProgramNodeProcessor(ExpressionProcessor, expr2, scope, ref e2);

            bool areEqual(object o1, object o2) 
            {
                if (o1 == null)
                {
                    return o2 == null;
                }

                if (o1 is bool)
                {
                    return Convert.ToBoolean(o1) == Convert.ToBoolean(o2);
                }

                if (o1 is Number)
                {
                    return (o1 as Number).DoubleValue == (o2 as Number).DoubleValue;
                }

                var method = o1.GetType().GetMethod("op_Equality");
                if (method == null)
                {
                    return o1 == o2;
                }

                return (bool) method.Invoke(null, new object[] { o1, o2 });
            }

            if (programNode.MatchChildren("Expression", "==", "Expression")) 
            {
                value = areEqual(e1, e2);
                return true;
            }

            if (programNode.MatchChildren("Expression", "!=", "Expression"))
            {
                value = !areEqual(e1, e2);
                return true;
            }

            if (programNode.MatchChildren("Expression", "<", "Expression"))
            {
                if (e1 is Number) {
                    value = (e1 as Number).DoubleValue < (e2 as Number).DoubleValue;
                    return true;
                }

                var method = e1.GetType().GetMethod("op_LessThan");
                value = method.Invoke(null, new object[] { e1, e2 });
                return true;
            }

            if (programNode.MatchChildren("Expression", ">", "Expression"))
            {
                if (e1 is Number)
                {
                    value = (e1 as Number).DoubleValue > (e2 as Number).DoubleValue;
                    return true;
                }

                var method = e1.GetType().GetMethod("op_GreaterThan");
                value = method.Invoke(null, new object[] { e1, e2 });
                return true;
            }

            if (programNode.MatchChildren("Expression", "<=", "Expression"))
            {
                if (e1 is Number)
                {
                    value = (e1 as Number).DoubleValue <= (e2 as Number).DoubleValue;
                    return true;
                }

                var method = e1.GetType().GetMethod("op_LessThanOrEqual");
                value = method.Invoke(null, new object[] { e1, e2 });
                return true;
            }

            if (programNode.MatchChildren("Expression", ">=", "Expression"))
            {
                if (e1 is Number)
                {
                    value = (e1 as Number).DoubleValue >= (e2 as Number).DoubleValue;
                    return true;
                }

                var method = e1.GetType().GetMethod("op_GreaterThanOrEqual");
                value = method.Invoke(null, new object[] { e1, e2 });
                return true;
            }

            throw new NotImplementedException();
        }
    }
}
