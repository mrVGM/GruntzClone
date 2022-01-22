using ScriptingLanguage.Parser;

namespace ScriptingLanguage.Interpreter
{
    class Function : IFunction
    {
        public ProgramNode Block;
        Scope ParentScope;
        public Scope ScopeTemplate 
        {
            get 
            {
                var scope = new Scope { ParentScope = ParentScope };
                foreach (var param in ParameterNames) 
                {
                    scope.AddVariable(param, null);
                }
                return scope;
            }
        }

        public Function(Scope parentScope) 
        {
            ParentScope = parentScope;
        }

        public string[] ParameterNames { get; set; }
        IProgramNodeProcessor BlockProcessor = new BlockProcessor(new OperationGroupProcessor { StopOnReturn = true });
        public object Execute(Scope scope)
        {
            object functionResult = null;
            var localScope = new Scope { ParentScope = scope };
            var res = NodeProcessor.ExecuteProgramNodeProcessor(BlockProcessor, Block, localScope, ref functionResult);
            var returnOperation = res as OperationProcessor.ReturnOperation;
            if (returnOperation != null && returnOperation.ReturnExpression) 
            {
                return functionResult;
            }
            return null;
        }
    }
}
