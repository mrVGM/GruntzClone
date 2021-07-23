using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base
{
    public class DefRepositoryDef : Def
    {
        [Serializable]
        private class DefWithId
        {
            public Def Def;
            public string Guid;
        }

        [SerializeField]
        private List<DefWithId> DefsWithId;

        public IEnumerable<Def> AllDefs => DefsWithId.Select(x => x.Def);

        internal object OfType<T>()
        {
            throw new NotImplementedException();
        }

        private void RegisterDef(Def def)
        {
            if (!DefsWithId.Any(x => x.Def == def))
            {
                DefsWithId.Add(new DefWithId { Def = def, Guid = Guid.NewGuid().ToString() });
            }
        }

        public void UpdateDefs(IEnumerable<Def> allDefs)
        {
            foreach (var def in allDefs)
            {
                RegisterDef(def);
            }
            DefsWithId.RemoveAll(x => x.Def == null);
        }

        public Def GetDefById(string id)
        {
            return DefsWithId.FirstOrDefault(x => x.Guid == id).Def;
        }

        public string GetDefId(Def def)
        {
            return DefsWithId.FirstOrDefault(x => x.Def == def).Guid;
        }
    }
}
