using Base;
using System;

namespace Gruntz.CollectedMaterialManager
{
    [Serializable]
    public class CollectedMaterialManagerData : ISerializedObjectData
    {
        public int NumberOfMaterialPiecesCollected = 0;
    }
}
