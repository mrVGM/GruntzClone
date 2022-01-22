using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ScriptingLanguage.Interpreter;

namespace ScriptingLanguage.BaseFunctions
{
    class CreateDelegateFunction : IFunction
    {
        const string delegateType = "delegate_type";
        const string functionToExecute = "function_to_execute";
        const string functionToExecuteId = "function_to_execute_id";

        string[] parameters = { delegateType, functionToExecute, functionToExecuteId };
        public Scope ScopeTemplate
        {
            get
            {
                var scope = new Scope();
                scope.AddVariable(delegateType, null);
                scope.AddVariable(functionToExecute, null);
                scope.AddVariable(functionToExecuteId, null);
                return scope;
            }
        }

        public string[] ParameterNames => parameters;

        private static object CallFunc(IFunction func, IEnumerable<object> args)
        {
            var argArray = args.ToArray();
            var scope = func.ScopeTemplate;

            var parameterNames = func.ParameterNames;
            for (int i = 0; i < parameterNames.Length; ++i) 
            {
                scope.SetVariable(parameterNames[i], argArray[i]);
            }

            return func.Execute(scope);
        }

        public static object CreateLambda(Type delegateType, IFunction func)
        {
            var parameters = delegateType.GetMethod("Invoke")
                .GetParameters()
                .Select(x => Expression.Parameter(x.ParameterType))
                .ToArray();

            var funcExpr = Expression.Constant(func);

            // Declare a paramArray parameter to use inside the Expression.Block
            var paramArray = Expression.Parameter(typeof(object[]), "paramArray");

            var method = typeof(CreateDelegateFunction).GetMethod("CallFunc", BindingFlags.NonPublic | BindingFlags.Static);

            var block = Expression.Block(
                new ParameterExpression[] { paramArray },
                Expression.Assign(
                    paramArray,
                    Expression.NewArrayInit(
                        typeof(object),
                        parameters.Select(p => Expression.Convert(p, typeof(object))))),

                Expression.Call(method, funcExpr, paramArray)
            );

            var lambda = Expression.Lambda(delegateType, block, parameters);
            var compiledLambda = lambda.Compile();
            return compiledLambda;
        }

        public object Execute(Scope scope)
        {
            var typeOfDelegate = scope.GetVariable(delegateType) as Type;
            var func = scope.GetVariable(functionToExecute) as IFunction;

            var lambda = CreateLambda(typeOfDelegate, func);

            return lambda;
        }
    }
}
