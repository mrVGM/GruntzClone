using System.IO;
using ScriptingLanguage.Interpreter;

namespace ScriptingLanguage.BaseFunctions
{
    class RequireFunction : IFunction
    {
        const string filename = "filename";
        const string exports = "exports";
        public Scope ScopeTemplate
        {
            get
            {
                var scope = new Scope { ParentScope = ContextScope };
                scope.AddVariable(filename, null);
                scope.AddVariable(exports, new GenericObject());

                var localScope = new Scope { ParentScope = scope };
                return localScope;
            }
        }
        public string[] ParameterNames { get; private set; } = { filename };

        private Session Session;
        private Scope ContextScope;
        
        public RequireFunction(Session session, Scope contextScope)
        {
            Session = session;
            ContextScope = contextScope;
        }

        public object Execute(Scope scope)
        {
            string scriptName = scope.GetVariable(filename) as string;
            var fullPath = Session.WorkingDir + scriptName;
            if (!File.Exists(fullPath)) 
            {
                throw new FileNotFoundException($"Cannot find script: {fullPath}!");
            }

            var interpreter = new Interpreter.Interpreter(Session);
            interpreter.RunScriptFile(fullPath, scope);
            return scope.GetVariable("exports");
        }
    }
}
