using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptingLanguage.Interpreter
{
    public class Scope
    {
        public Scope ParentScope;
        Dictionary<string, object> variables = new Dictionary<string, object>();

        public bool HasVariable(string name, bool inCurrentScope = false) 
        {
            if (variables.ContainsKey(name)) 
            {
                return true;
            }
            if (!inCurrentScope) 
            {
                if (ParentScope != null) 
                {
                    return ParentScope.HasVariable(name, inCurrentScope);
                }
            }
            return false;
        }
        public object GetVariable(string name, bool InCurrentScope = false) 
        {
            object res = null;
            if (variables.TryGetValue(name, out res)) 
            {
                return res;
            }
            if (ParentScope != null) 
            {
                return ParentScope.GetVariable(name);
            }
            return null;
        }

        public void SetVariable(string name, object value)
        {
            if (variables.ContainsKey(name))
            {
                variables[name] = value;
                return;
            }
            if (ParentScope != null)
            {
                ParentScope.SetVariable(name, value);
            }
            else 
            {
                throw new Exception("Variable not declared!");
            }
        }
        public void AddVariable(string name, object value)
        {
            variables[name] = value;
        }
    }
}
