using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptingLanguage.Interpreter
{
    public class GenericObject
    {
        public class ObjectContainer 
        {
            public object ObjectValue;
        }

        private Dictionary<string, ObjectContainer> properties = new Dictionary<string, ObjectContainer>();
        public ObjectContainer GetPropoerty(string name) 
        {
            ObjectContainer val = null;
            if (!properties.TryGetValue(name, out val)) 
            {
                val = new ObjectContainer();
                properties[name] = val;
            }
            return val;
        }
    }
}
