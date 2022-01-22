using System;
using System.Collections.Generic;
using System.Linq;
using ScriptingLanguage.Interpreter;

namespace ScriptingLanguage.BaseFunctions
{
    public class CreateObjectFunction : IFunction
    {
        const string typeNameParam = "type_name";
        const string argTypesParam = "args_types";
        const string argValuesParam = "args_values";
        string[] args = { typeNameParam, argTypesParam, argValuesParam};
        public Scope ScopeTemplate 
        {
            get 
            {
                var scope = new Scope();
                scope.AddVariable(typeNameParam, null);
                scope.AddVariable(argTypesParam, null);
                scope.AddVariable(argValuesParam, null);
                return scope;
            }
        }

        public string[] ParameterNames => args;

        public object Execute(Scope scope)
        {
            var type = Utils.GetTypeAcrossAssemblies(scope.GetVariable(typeNameParam) as string);
            var argTypes = (scope.GetVariable(argTypesParam) as IEnumerable<object>).Select(x => Utils.GetTypeAcrossAssemblies(x as string)).ToArray();
            var args = (scope.GetVariable(argValuesParam) as IEnumerable<object>).ToArray();

            for (int i = 0; i < argTypes.Length; ++i) 
            {
                if (args[i] == null) 
                {
                    continue;
                }
                var cur = args[i];
                if (!argTypes[i].IsAssignableFrom(cur.GetType())) 
                {
                    args[i] = Convert.ChangeType(cur, argTypes[i]);
                }
            }
            var obj = Activator.CreateInstance(type, args);
            return obj;
        }
    }
}
