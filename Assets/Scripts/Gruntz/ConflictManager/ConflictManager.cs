using Base;
using Base.Actors;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.ConflictManager
{
    public class ConflictManager : IContextObject
    {
        public interface ILock { }
        private struct Lock : ILock
        {
            private ConflictManager _conflictManager;
            private Conflict _conflict;
            public Lock(ConflictManager conflictManager, Actor attacker, Actor target)
            {
                _conflictManager = conflictManager;
                _conflict = new Conflict { Attacker = attacker, Target = target };
                _conflictManager._openConflicts.Add(_conflict);
            }

            public void Unlock()
            {
                _conflictManager._openConflicts.Remove(_conflict);
            }
        }

        private class Conflict
        {
            public Actor Attacker;
            public Actor Target;

            public bool IsInTheConflict(Actor actor)
            {
                return Attacker == actor || Target == actor;
            }
        }
        private List<Conflict> _openConflicts = new List<Conflict>();

        public ILock TryGetLock(Actor attacker, Actor target)
        {
            if (_openConflicts.Any(x => x.IsInTheConflict(attacker) || x.IsInTheConflict(target))) {
                return null;
            }

            return new Lock(this, attacker, target);
        }

        public void ReturnLock(ILock lockToReturn)
        {
            var l = (Lock)lockToReturn;
            l.Unlock();
        }

        public void DisposeObject()
        {
        }

        public static ConflictManager GetConflictManagerFromContext()
        {
            var game = Game.Instance;
            var coinflictManagerDef = game.DefRepositoryDef.AllDefs.OfType<ConflictManagerDef>().FirstOrDefault();
            var conflictManager = game.Context.GetRuntimeObject(coinflictManagerDef) as ConflictManager;
            return conflictManager;
        }
    }
}
