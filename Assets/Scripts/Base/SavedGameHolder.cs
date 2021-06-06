using System;

namespace Base
{
    public class SavedGameHolder : IContextObject
    {
        public SavedGame SavedGame;
        public void DisposeObject()
        {
        }
    }
}
