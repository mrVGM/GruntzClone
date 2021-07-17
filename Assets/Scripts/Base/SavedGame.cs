using System;
using System.Collections.Generic;

namespace Base
{
    [Serializable]
    public class SavedGame
    {
        [Serializable]
        public class SerializedContextObject
        {
            public DefRef<Def> Def;
            public ISerializedObjectData ContextObjectData;
        }
        public DefRef<LevelDef> Level;
        public List<SerializedContextObject> SerializedContextObjects;
    }
}
