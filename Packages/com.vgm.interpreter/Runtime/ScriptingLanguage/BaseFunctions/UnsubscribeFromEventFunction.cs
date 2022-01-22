using System;
using ScriptingLanguage.Interpreter;

namespace ScriptingLanguage.BaseFunctions
{
    class UnsubscribeFromEventFunction : IFunction
    {
        const string targetObject = "target_object";
        const string eventName = "event_name";
        const string delegateFunction = "delegate_function";

        string[] parameters = { targetObject, eventName, delegateFunction };
        public Scope ScopeTemplate
        {
            get
            {
                var scope = new Scope();
                scope.AddVariable(targetObject, null);
                scope.AddVariable(eventName, null);
                scope.AddVariable(delegateFunction, null);
                return scope;
            }
        }

        public string[] ParameterNames => parameters;

        public object Execute(Scope scope)
        {
            var objectType = scope.GetVariable(targetObject).GetType();
            var eventInfo = objectType.GetEvent(scope.GetVariable(eventName) as string);
            eventInfo.RemoveEventHandler(scope.GetVariable(targetObject), scope.GetVariable(delegateFunction) as Delegate);
            return null;
        }
    }
}
