using System;
using System.Linq;

namespace Base
{
    public class SavedGameHolder : IContextObject
    {
        public SavedGame SavedGame;
        public void DisposeObject()
        {
        }

        public static SavedGameHolder GetSavedGameHolderFromContext()
        {
            var game = Game.Instance;
            var savedGameHolderDef = game.DefRepositoryDef.AllDefs.OfType<SavedGameHolderDef>().First();
            return game.Context.GetRuntimeObject(savedGameHolderDef) as SavedGameHolder;
        }
    }
}
