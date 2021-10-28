using Base;
using Base.Actors;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.ConflictManager
{
    public class ConflictManager : IContextObject
    {
        public interface ILock { }
        private class Lock : ILock
        {
            public Conflict Conflict { get; }
            public Lock(Conflict conflict)
            {
                Conflict = conflict;
            }
        }
        public class Conflict
        {
            public HashSet<Actor> Participants = new HashSet<Actor>();
            private Lock _lock;
            public ILock ConflictLock
            {
                get
                {
                    if (_lock == null) {
                        return null;
                    }

                    var tmp = _lock;
                    _lock = null;
                    return tmp;
                }
                set
                {
                    _lock = value as Lock;
                }
            }

            public Conflict()
            {
                _lock = new Lock(this);
            }
        }

        private List<Conflict> OpenConflicts = new List<Conflict>();

        public ILock TryGetLock(Actor attacker, Actor target)
        {
            var existingConflict = OpenConflicts
                .FirstOrDefault(x => x.Participants.Contains(target));

            if (existingConflict == null) {
                existingConflict = new Conflict();
                OpenConflicts.Add(existingConflict);
                existingConflict.Participants.Add(target);
            }

            existingConflict.Participants.Add(attacker);

            return existingConflict.ConflictLock;
        }

        public void ReturnLock(ILock lockToReturn)
        {
            var l = lockToReturn as Lock;
            l.Conflict.ConflictLock = lockToReturn;
        }

        public void LeaveConflict(Actor actor)
        {
            var cache = OpenConflicts.ToList();
            foreach (var conflict in cache) {
                conflict.Participants.Remove(actor);
                if (conflict.Participants.Count() <= 1) {
                    OpenConflicts.Remove(conflict);
                }
            }
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
