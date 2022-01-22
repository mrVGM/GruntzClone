using System.Collections;
using ScriptingLanguage.Interpreter;

namespace ScriptingLanguage.BaseFunctions
{
    class CreateCoroutineFunction : IFunction
    {
        const string funcParam = "func_param";
        string[] parameters = { funcParam };
        public Scope ScopeTemplate
        {
            get 
            {
                var scope = new Scope();
                scope.AddVariable(funcParam, null);
                return scope;
            }
        }

        public string[] ParameterNames => parameters;

        public object Execute(Scope scope)
        {
            var func = scope.GetVariable(funcParam) as IFunction;

            IEnumerator crt() 
            {
                var curFunc = func;
                while (curFunc != null)
                {
                    var template = curFunc.ScopeTemplate;
                    curFunc = curFunc.Execute(template) as IFunction;
                    yield return null;
                }
            }

            return crt();
        }
    }
}
