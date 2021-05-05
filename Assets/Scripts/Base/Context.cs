using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public class Context : MonoBehaviour
    {
        private Dictionary<Def, IContextObject> runtimeIntances { get; } = new Dictionary<Def, IContextObject>();
        public IContextObject GetRuntimeObject(Def def)
        {
            IContextObject res = null;
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
            foreach (var pair in runtimeIntances)
            {
                pair.Value.DisposeObject();
            }
            runtimeIntances.Clear();
        }
    }
}
