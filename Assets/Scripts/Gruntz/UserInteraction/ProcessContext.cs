using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.UserInteraction
{
    public class ProcessContext : MonoBehaviour
    {
        private Dictionary<ProcessContextTagDef, object> context = new Dictionary<ProcessContextTagDef, object>();
        public void PutItem(ProcessContextTagDef tag, object item)
        {
            context[tag] = item;
        }
        public object GetItem(ProcessContextTagDef tag)
        {
            object res = null;
            if (context.TryGetValue(tag, out res))
            {
                return res;
            }
            return null;
        }
    }
}
