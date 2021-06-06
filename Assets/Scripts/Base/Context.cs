using System.Collections.Generic;
using UnityEngine;
using static Base.SavedGame;

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

        public IEnumerable<SerializedContextObject> GetSerializedContextObjects()
        {
            foreach (var pair in runtimeIntances)
            {
                var serializedObject = pair.Value as ISerializedObject;

                if (serializedObject != null)
                {
                    yield return new SerializedContextObject { Def = pair.Key, ContextObjectData = serializedObject.Data };
                }
            }
        }
    }
}
