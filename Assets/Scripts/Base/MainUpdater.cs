using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base
{
    public class MainUpdater : MonoBehaviour
    {
        private HashSet<IOrderedUpdate> updatableItems { get; } = new HashSet<IOrderedUpdate>();
        private Dictionary<ExecutionOrderTagDef, List<IOrderedUpdate>> sortedUpdatables { get; } = new Dictionary<ExecutionOrderTagDef, List<IOrderedUpdate>>();

        public ExecutionOrderTagDef[] ExecutionOrder;

        public void RegisterUpdatable(IOrderedUpdate updatable)
        {
            updatableItems.Add(updatable);
        }
        public void UnRegisterUpdatable(IOrderedUpdate updatable)
        {
            updatableItems.Remove(updatable);
        }

        IEnumerator<object> UpdateCrt()
        {
            while (true)
            {
                foreach (var tagDef in ExecutionOrder)
                {
                    List<IOrderedUpdate> tmp = null;
                    if (sortedUpdatables.TryGetValue(tagDef, out tmp))
                    {
                        tmp.Clear();
                    }
                    else
                    {
                        tmp = new List<IOrderedUpdate>();
                        sortedUpdatables[tagDef] = tmp;
                    }
                    tmp.AddRange(updatableItems.Where(x => x.OrderTagDef == tagDef));
                }

                foreach (var tagDef in ExecutionOrder)
                {
                    foreach (var updatable in sortedUpdatables[tagDef])
                    {
                        updatable.DoUpdate();
                    }
                }
                yield return null;
            }
        }
        IEnumerator<object> crt;
        void Update()
        {
            if (crt == null) {
                crt = UpdateCrt();
            }
            crt.MoveNext();
        }
    }
}
