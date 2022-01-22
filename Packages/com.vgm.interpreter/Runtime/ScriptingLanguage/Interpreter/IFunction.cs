namespace ScriptingLanguage.Interpreter
{
    public interface IFunction
    {
        Scope ScopeTemplate { get; }
        string[] ParameterNames { get; }
        object Execute(Scope scope);
    }
}
