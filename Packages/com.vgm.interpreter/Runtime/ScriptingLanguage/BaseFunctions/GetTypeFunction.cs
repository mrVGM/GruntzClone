using ScriptingLanguage.Interpreter;

namespace ScriptingLanguage.BaseFunctions
{
    class GetTypeFunction : IFunction
    {
        const string typeName = "type_name";

        string[] parameters = { typeName };
        public Scope ScopeTemplate
        {
            get
            {
                var scope = new Scope();
                scope.AddVariable(typeName, null);
                return scope;
            }
        }

        public string[] ParameterNames => parameters;

        public object Execute(Scope scope)
        {
            string name = scope.GetVariable(typeName) as string;
            return Utils.GetTypeAcrossAssemblies(name);
        }
    }
}
