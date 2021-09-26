using System;
using UnityEngine;

namespace Base
{
    public class Def : ScriptableObject
    {
        public DefRef<T> ToDefRef<T>() where T : Def
        {
            var defRepo = Game.Instance.DefRepositoryDef;
            return new DefRef<T> { Guid = defRepo.GetDefId(this) };
        }
    }

    [Serializable]
    public struct DefRef<T> where T : Def
    {
        public string Guid;
        [NonSerialized]
        private T def;
        public static implicit operator T(DefRef<T> defRef)
        {
            if (defRef.Equals(default(DefRef<T>))) {
                return null;
            }
            if (defRef.def != null)
            {
                return defRef.def;
            }

            var defRepo = Game.Instance.DefRepositoryDef;
            defRef.def = defRepo.GetDefById(defRef.Guid) as T;
            return defRef.def;
        }
    }
}
