using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base
{
    public enum MainUpdaterUpdateTime
    {
        FixedUpdate,
        FixedCrt,
        Update,
    }

    public class MainUpdaterLock
    {
        public interface ILock { }
        private class Lock : ILock
        {
            public ExecutionOrderTagDef Tag;
        }

        Dictionary<ExecutionOrderTagDef, ILock> _locks = new Dictionary<ExecutionOrderTagDef, ILock>(); 

        public ILock TryLock(ExecutionOrderTagDef tag)
        {
            ILock l = null;
            if (!_locks.TryGetValue(tag, out l))
            {
                l = new Lock { Tag = tag };
                _locks[tag] = l;
            }
            _locks[tag] = null;
            return l;
        }

        public void Unlock(ILock l)
        {
            Lock tmp = l as Lock;
            _locks[tmp.Tag] = l;
        }

        public override string ToString()
        {
            string res = "";
            foreach (var l in _locks)
            {
                res += $"{l.Key.name} : {(l.Value == null ? "Locked" : "Unlocked")}\n";
            }
            return res;
        }
    }

    public class MainUpdater : MonoBehaviour
    {
        private HashSet<IOrderedUpdate> updatableItems { get; } = new HashSet<IOrderedUpdate>();
        private Dictionary<ExecutionOrderTagDef, List<IOrderedUpdate>> sortedUpdatables { get; } = new Dictionary<ExecutionOrderTagDef, List<IOrderedUpdate>>();

        public float FixedDelta = 0.01f;

        public ExecutionOrderTagDef[] ExecutionOrder;
        public MainUpdaterLock MainUpdaterLock { get; } = new MainUpdaterLock();

        public void RegisterUpdatable(IOrderedUpdate updatable)
        {
            updatableItems.Add(updatable);
        }
        public void UnRegisterUpdatable(IOrderedUpdate updatable)
        {
            updatableItems.Remove(updatable);
        }

        private void CacheCurrentUpdatables()
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
        }

        private void UpdateUpdateables(MainUpdaterUpdateTime mainUpdaterUpdateTime)
        {
            CacheCurrentUpdatables();
            var tagDefs = ExecutionOrder.Where(x => x.UpdateTime.Contains(mainUpdaterUpdateTime));

            foreach (var tag in tagDefs) {
                var l = MainUpdaterLock.TryLock(tag);
                if (l == null)
                {
                    return;
                }
                var updatables = sortedUpdatables[tag];
                foreach (var updatable in updatables)
                {
                    updatable.DoUpdate();
                }
                MainUpdaterLock.Unlock(l);
            }
        }

        IEnumerator<object> UpdateCrt()
        {
            while (true)
            {
                UpdateUpdateables(MainUpdaterUpdateTime.Update);
                yield return null;
            }
        }
        IEnumerator<object> _updateCrt;
        IEnumerator<object> _fixedCrt;
        IEnumerator<object> _fixedUpdateCrt;

        void Update()
        {
            if (_updateCrt == null) {
                _updateCrt = UpdateCrt();
            }
            _updateCrt.MoveNext();
        }

        private void FixedUpdate()
        {
            if (_fixedUpdateCrt == null)
            {
                _fixedUpdateCrt = FixedUpdateCrt();
            }
            _fixedUpdateCrt.MoveNext();
        }

        private void Awake()
        {
            Time.fixedDeltaTime = FixedDelta;
            _fixedCrt = FixedCrt();
            StartCoroutine(_fixedCrt);
        }

        private IEnumerator<object> FixedCrt()
        {
            while (true)
            {
                UpdateUpdateables(MainUpdaterUpdateTime.FixedCrt);
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator<object> FixedUpdateCrt()
        {
            while (true)
            {
                UpdateUpdateables(MainUpdaterUpdateTime.FixedUpdate);
                yield return null;
            }
        }
    }
}
