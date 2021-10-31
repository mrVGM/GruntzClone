using Base;
using System.Linq;

namespace Gruntz.CollectedMaterialManager
{
    public class CollectedMaterialManager : IContextObject, ISerializedObject
    {
        public CollectedMaterialManagerDef CollectedMaterialManagerDef { get; private set; }
        private CollectedMaterialManagerData _collectedMaterialManagerData = new CollectedMaterialManagerData();
        public ISerializedObjectData Data
        {
            get
            {
                return _collectedMaterialManagerData;
            }
            set
            {
                _collectedMaterialManagerData = value as CollectedMaterialManagerData;
            }
        }

        public int MaterialPiecesCollected => _collectedMaterialManagerData.NumberOfMaterialPiecesCollected;

        public void MaterialCollected()
        {
            ++_collectedMaterialManagerData.NumberOfMaterialPiecesCollected;
        }

        public void DisposeObject()
        {
        }

        public static CollectedMaterialManager GetCollectedMaterialManager()
        {
            var game = Game.Instance;
            var collectedMaterialManagerDef = game.DefRepositoryDef.AllDefs.OfType<CollectedMaterialManagerDef>().First();
            var context = game.Context;
            var abilityManager = context.GetRuntimeObject(collectedMaterialManagerDef) as CollectedMaterialManager;
            abilityManager.CollectedMaterialManagerDef = collectedMaterialManagerDef;
            return abilityManager;
        }
    }
}
