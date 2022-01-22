using System.Collections;
using ScriptingLanguage.Interpreter;

namespace ScriptingLanguage.BaseFunctions
{
    class ForeachFunction : IFunction
    {
        const string argCollection = "collection";
        const string argProcessFunction = "process_function";
        public Scope ScopeTemplate
        {
            get 
            {
                var scope = new Scope();
                scope = new Scope();
                scope.AddVariable(argCollection, null);
                scope.AddVariable(argProcessFunction, null);
                
                return scope;
            }
        }

        public string[] ParameterNames { get; private set; } = { argCollection, argProcessFunction };

        public object Execute(Scope scope)
        {
            var col = scope.GetVariable(argCollection, InCurrentScope: true) as IEnumerable;
            var processor = scope.GetVariable(argProcessFunction, InCurrentScope: true) as IFunction;

            foreach (var obj in col) 
            {
                var funcScope = processor.ScopeTemplate;
                funcScope.AddVariable(processor.ParameterNames[0], obj);
                processor.Execute(funcScope);
            }

            return null;
        }
    }
}
