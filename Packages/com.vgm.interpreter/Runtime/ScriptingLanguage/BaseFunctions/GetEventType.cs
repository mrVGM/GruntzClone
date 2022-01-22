using System;
using System.Collections.Generic;
using System.Text;
using ScriptingLanguage.Interpreter;

namespace ScriptingLanguage.BaseFunctions
{
    class GetEventType : IFunction
    {
        const string targetObject = "target_object";
        const string eventName = "event_name";

        string[] parameters = { targetObject, eventName };
        public Scope ScopeTemplate
        {
            get
            {
                var scope = new Scope();
                scope.AddVariable(targetObject, null);
                scope.AddVariable(eventName, null);
                return scope;
            }
        }

        public string[] ParameterNames => parameters;

        public object Execute(Scope scope)
        {
            var objectType = scope.GetVariable(targetObject).GetType();
            var eventInfo = objectType.GetEvent(scope.GetVariable(eventName) as string);
            var eventHandlerType = eventInfo.EventHandlerType;
            return eventHandlerType;
        }
    }
}
