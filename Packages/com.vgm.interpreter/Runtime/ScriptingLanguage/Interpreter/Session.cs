using ScriptingLanguage.BaseFunctions;

namespace ScriptingLanguage.Interpreter
{
    public class Session
    {
        public string WorkingDir { get; private set; }
        private Scope sessionScope { get; }
        public Parser.Parser Parser { get; private set; }

        private Scope workingScope = null;
        
        public Scope GetWorkingScope()
        {
            if (workingScope == null) {
                workingScope = new Scope { ParentScope = sessionScope };
            }

            return workingScope;
        }

        public class SessionFunc 
        {
            public string Name;
            public IFunction Function;
        }

        public Session(string workingDir, params SessionFunc[] additionalFunctions)
        {
            WorkingDir = workingDir;
            var interpteterScope = Interpreter.GetStaticScope();
            sessionScope = new Scope();
            sessionScope.ParentScope = interpteterScope;
            sessionScope.AddVariable("require", new RequireFunction(this, sessionScope));

            foreach (var func in additionalFunctions) 
            {
                sessionScope.AddVariable(func.Name, func.Function);
            }
        }

        public Session(string workingDir, Parser.Parser parser, params SessionFunc[] additionalFunctions) : this(workingDir, additionalFunctions)
        {
            Parser = parser;
        }

        public void Reset(string workingDir)
        {
            WorkingDir = workingDir;
            workingScope = null;
        }
    }
}
