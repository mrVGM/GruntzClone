using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public class Context : MonoBehaviour
    {
        private Dictionary<Def, object> runtimeIntances { get; } = new Dictionary<Def, object>();
        public object GetRuntimeObject(Def def)
        {
            object res = null;
            if (runtimeIntances.TryGetValue(def, out res)) 
            {
                return res;
            }
            var runtimeInstance = def as IRuntimeInstance;
            if (runtimeInstance == null) 
            { 
                return null;
            }
            res = runtimeInstance.CreateRuntimeInstance();
            runtimeIntances[def] = res;
            return res;
        }
        public void ClearContext()
        {
            runtimeIntances.Clear();
        }
    }
}
