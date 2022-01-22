using System;

namespace ScriptingLanguage.REPL
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MetaCommandAttribute : Attribute
    {
        public string Name;
    }
}
