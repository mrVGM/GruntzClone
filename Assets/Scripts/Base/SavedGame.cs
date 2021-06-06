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
            public Def Def;
            public ISerializedObjectData ContextObjectData;
        }
        public LevelDef Level;
        public List<SerializedContextObject> SerializedContextObjects;
    }
}